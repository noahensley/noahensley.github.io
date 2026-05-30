/*
 * File: runtime.c
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate the following functions
 * on April 6, 2026:
 *      newline()
 *      putv()
 *      getc()
 *
 * Claude (Anthropic) was used to fix the following bugs on April 6, 2026:
 *      Moved all forward declarations to the top of the file, before first use
 *      Fixed Handle -> HANDLE in forward declarations for GetStdHandle,
 *          CloseHandle, and WriteFile
 *      Replaced NULL with void* in WriteFile's last parameter
 *      Added missing forward declaration for ReadFile
 *      Added forward declarations for toHex, toDecimal, and toBin before putv
 *      Fixed ptr -> sptr in putc
 *      Fixed int64_t -> uint64_t in toDecimal
 *
 * Claude (Anthropic) was used to add forward declarations on April 18, 2026:
 *      Added forward declarations for all internal runtime functions
 *      Added _Static_assert for int64_t
 *      Added implementations for do_memory_alloc, outOfMemory, and prepend
 *      Added heap initialization in rtinit
 */

#define NULL 0
#define MEM_COMMIT 0x1000
#define MEM_RESERVE 0x2000
#define MEM_RELEASE 0x8000
#define PAGE_READWRITE 0x4
#define PAGE_READONLY 0x2
#define STORAGE_CLASS_HEAP 2
#define HEAP_SIZE (1<<20)

typedef unsigned int uint32_t;
typedef unsigned long long uint64_t;
typedef signed long long int64_t;

_Static_assert(sizeof(uint32_t) == 4, "Bad size for uint32_t");
_Static_assert(sizeof(uint64_t) == 8, "Bad size for uint64_t");
_Static_assert(sizeof(int64_t)  == 8, "Bad size for int64_t");

typedef uint32_t DWORD;
typedef void* HANDLE;
typedef int BOOL;

typedef struct String_ {
    uint64_t length;
    char data[1]; //length is bogus
} String;

typedef struct StackArg_ {
    uint64_t value;
    uint64_t storageClass;
} StackArg;

typedef struct Variable_ {
    uint64_t value;
    uint64_t storageClass;
} Variable;

typedef struct Block_{
    struct Block_* prev;        // doubly linked list
    struct Block_* next;        // doubly linked list
    uint64_t size;              // size in bytes of this block, including Block header
    uint64_t mark;              // not used by memory allocator
} Block;

/* Forward declarations for Win32 API functions.
 * These are resolved by the linker against kernel32.lib.
 * Declared before use since clang does not hoist function declarations. */
__attribute__((ms_abi)) HANDLE GetStdHandle(HANDLE h);
__attribute__((ms_abi)) void CloseHandle(HANDLE h);
__attribute__((ms_abi)) int WriteFile(HANDLE h, void* ptr, DWORD count, DWORD* written, void* overlap);
__attribute__((ms_abi)) int ReadFile(HANDLE h, void* ptr, DWORD count, DWORD* read, void* overlap);
__attribute__((ms_abi)) void* VirtualAlloc(void* addr, uint64_t size, uint32_t type, uint32_t perms);

/* Forward declarations for conversion helpers. */
unsigned toHex(uint64_t x, char output[16]);
unsigned toDecimal(uint64_t x, char output[20]);
unsigned toBin(uint64_t number, char output[64]);

/* Forward declarations for I/O functions. */
__attribute__((ms_abi)) void rtinit();
__attribute__((ms_abi)) void rtcleanup();
__attribute__((ms_abi)) uint64_t putc(StackArg* sptr);
__attribute__((ms_abi)) uint64_t newline(StackArg* sptr);
__attribute__((ms_abi)) uint64_t putv(StackArg* sptr);
__attribute__((ms_abi)) uint64_t getc(StackArg* sptr);
__attribute__((ms_abi)) void print(StackArg* sptr);
__attribute__((ms_abi)) uint64_t length(StackArg* sptr);
__attribute__((ms_abi)) String* concatenateStrings(String* s1, String* s2, uint64_t* rsp, uint64_t* rbp);
__attribute__((ms_abi)) String* intToString(int64_t number, uint64_t* rsp, uint64_t* rbp);

/* Forward declarations for memory management. */
static void* memory_alloc(uint64_t* rsp, uint64_t* rbp, uint64_t size);
static void* do_memory_alloc(uint64_t size);
static void outOfMemory();
void memory_free(Block* f);
void insertAfter(Block* toInsert, Block* afterThis);
static void prepend(Block* f);
int blocksAreContiguous(Block* a, Block* b);
void combine(Block* a, Block* b);
static void fatal();

/* Forward declarations for garbage collector. */
static Block* heapToBlock(void* p);
static uint64_t markGlobals(uint64_t currentMark, void* startAddr, void* endAddr);
static uint64_t markLocals(uint64_t currentMark, uint64_t* rsp, uint64_t* rbp);
uint64_t sweep(uint64_t currentMark);
void markAndSweep(uint64_t* rsp, uint64_t* rbp);

/* Forward declarations for debug helpers. */
static void debugputc(char c);
static void debugprints(const char* x);
static void debugprinti(int64_t x);

/* Extern labels emitted by the compiler for GC root scanning. */
extern void* dataStart;
extern void* dataEnd;
extern void* bssStart;
extern void* bssEnd;

static HANDLE stdin;
static HANDLE stdout;
static HANDLE stderr;
static Block* head;
char* heap;

/* rtinit - initializes stdin, stdout, and stderr handles.
 * Must be called before any I/O functions are used.
 * Called from _start in the generated assembly before main. */
__attribute__((ms_abi)) void rtinit()
{
    /*
     * Casting the 32-bit constant directly to void* could leave the upper 32 bits undefined on a 64-bit platform.
     *  Going through uint64_t first ensures the value is properly zero-extended to 64 bits before being treated
     *   as a pointer. */
    stdin  = GetStdHandle((HANDLE)(uint64_t)0xfffffff6);
    stdout = GetStdHandle((HANDLE)(uint64_t)0xfffffff5);
    stderr = GetStdHandle((HANDLE)(uint64_t)0xfffffff4);
    //a 1MB heap; initialize as a single free block covering the entire heap
    heap = VirtualAlloc(0, HEAP_SIZE, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
    Block* initialBlock = (Block*) heap;
    initialBlock->size = HEAP_SIZE;
    initialBlock->mark = 0;
    initialBlock->prev = NULL;
    initialBlock->next = NULL;
    head = initialBlock;
}

/* rtcleanup - closes stdin, stdout, and stderr handles.
 * Must be called after main returns, before ExitProcess. */
__attribute__((ms_abi)) void rtcleanup()
{
    CloseHandle(stdin);
    CloseHandle(stdout);
    CloseHandle(stderr);
}

/* putc - writes a single character to stdout.
 * sptr[0].value: ASCII value of the character to write.
 * Returns 1 on completion. */
__attribute__((ms_abi)) uint64_t putc(StackArg* sptr)
{
    DWORD count;
    char c = sptr[0].value;
    WriteFile(stdout, &c, 1, &count, NULL);
    return 1;
}

/* newline - writes a newline character (ASCII 10) to stdout.
 * Takes no meaningful arguments. Returns 0 (void). */
__attribute__((ms_abi)) uint64_t newline(StackArg* sptr)
{
    StackArg arg;
    arg.value = 10;
    arg.storageClass = 0;
    putc(&arg);
    return 0;
}

/* putv - prints integer x to stdout in base y.
 * sptr[0].value: the integer to print.
 * sptr[1].value: the base to use; must be 2, 10, or 16.
 * Returns 1 on success, 0 if y is not a supported base. */
__attribute__((ms_abi)) uint64_t putv(StackArg* sptr)
{
    uint64_t x = sptr[0].value;
    uint64_t y = sptr[1].value;

    char output[64];
    unsigned len;

    if (y == 2) {
        len = toBin(x, output);
    } else if (y == 10) {
        len = toDecimal(x, output);
    } else if (y == 16) {
        len = toHex(x, output);
    } else {
        return 0;
    }

    StackArg arg;
    arg.storageClass = 0;
    for (unsigned i = 0; i < len; i++) {
        arg.value = output[i];
        putc(&arg);
    }

    return 1;
}

/* getc - reads a single character from stdin.
 * Takes no meaningful arguments.
 * Returns the ASCII value of the character read. */
__attribute__((ms_abi)) uint64_t getc(StackArg* sptr)
{
    DWORD count;
    char c;
    ReadFile(stdin, &c, 1, &count, NULL);
    return (uint64_t)c;
}

__attribute__((ms_abi)) void print(StackArg* sptr)
{
    String* s = (String*)sptr[0].value;
    DWORD count;
    for (uint64_t i = 0; i < s->length; i++)
    {
        WriteFile(stdout, s->data+i, 1, &count, NULL);
    }
}

__attribute__((ms_abi)) uint64_t length(StackArg* sptr)
{
    String* s = (String*)sptr[0].value;
    return s->length;
}

__attribute__((ms_abi)) String* concatenateStrings(String* s1, String* s2, uint64_t* rsp, uint64_t* rbp)
{
    uint64_t newLen = s1->length + s2->length;
    String* s = (String*) memory_alloc(rsp, rbp, sizeof(uint64_t) + newLen + 1);

    for (uint64_t i = 0; i < s1->length; i++)
        s->data[i] = s1->data[i];

    uint64_t b = s1->length;
    for (uint64_t i = 0; i < s2->length; i++)
        s->data[b+i] = s2->data[i];

    s->length = newLen;
    s->data[newLen] = '\0';

    return s;
}

__attribute__((ms_abi)) String* intToString(int64_t number, uint64_t* rsp, uint64_t* rbp)
{
    char x[32];
    unsigned count = toDecimal(number, x);
    String* s = (String*) memory_alloc(rsp, rbp, count + 8);
    if (!s)
        fatal();
    s->length = count;
    for (unsigned i = 0; i < count; i++)
        s->data[i] = x[i];
    return s;
}

/* toHex - converts x to a hexadecimal string stored in output.
 * Leading zeros are suppressed. Returns the number of characters written. */
unsigned toHex(uint64_t x, char output[16]){
    unsigned shiftcount = 60;
    unsigned oo=0;
    const char* digits = "0123456789abcdef";
    if( x == 0 ){
        output[0]='0';
        return 1;
    }
    for(unsigned i=0;i<16;++i){
        unsigned j = (unsigned)((x>>shiftcount) & 0xf );
        if( oo > 0 || j )
            output[oo++] = digits[j];
        shiftcount -= 4;
    }
    return oo;
}

/* toDecimal - converts x to a decimal string stored in output.
 * Leading zeros are suppressed. Returns the number of characters written.
 * Supports values up to 2^64 - 1. */
unsigned toDecimal(uint64_t x, char output[20])
{
    if( x == 0 ){
        *output = '0';
        return 1;
    }
    uint64_t place = 10000000000000000000ULL;
    int oo=0;
    while(place > 0 ){
        uint64_t quotient = x/place;
        if( quotient || oo > 0 ) {
            output[oo++] = '0' + quotient;
        }
        x = x - quotient * place;
        place = place/10;
    }

    return oo;
}

/* toBin - converts number to a binary string stored in output.
 * Leading zeros are suppressed. Returns the number of characters written. */
unsigned toBin(uint64_t number, char output[64]){
    uint64_t mask = 0x8000000000000000ULL;
    if( number == 0 ){
        output[0]=0;
        return 1;
    }
    int oo=0;
    for(int i=0;i<64;++i,mask>>=1){
        if( mask & number ){
            output[oo++] = '1';
        } else {
            if( oo > 0 )
                output[oo++] = '0';
        }
    }
    return oo;
}

/* heapToBlock - returns a pointer to the Block header for a heap allocation. */
static Block* heapToBlock(void* p){
    char* c = (char*) p;
    c -= sizeof(Block);
    return (Block*) c;
}

/* markGlobals - marks all heap-allocated globals in [startAddr, endAddr).
 * Returns the total bytes of live heap data found. */
static uint64_t markGlobals(uint64_t currentMark, void* startAddr, void* endAddr){
    uint64_t allocated=0;
    Variable* p = (Variable*) startAddr;
    Variable* end = (Variable*) endAddr;
    while(p<end){
        if( p->storageClass == STORAGE_CLASS_HEAP ){
            Block* b = heapToBlock((void*)(p->value));
            b->mark = currentMark;
            allocated += b->size;
        }
        p++;
    }
    return allocated;
}

/* markLocals - walks the call stack from rsp/rbp marking all heap-allocated locals.
 * Returns the total bytes of live heap data found. */
static uint64_t markLocals(uint64_t currentMark,
                    uint64_t* rsp, uint64_t* rbp){
    uint64_t allocated=0;
    Variable* p = (Variable*) rsp;
    while(rbp){
        while( (char*)p < (char*)rbp ){
            if( p->storageClass == STORAGE_CLASS_HEAP ){
                Block* b = heapToBlock((void*)p->value);
                b->mark = currentMark;
                allocated += b->size;
            }
            p++;
        }
        uint64_t saved_rbp = *rbp;
        p = (Variable*)(rbp+2);  // skip saved rbp and return address
        rbp = (uint64_t*) saved_rbp;
    }
    return allocated;
}

/* sweep - frees all blocks that were not reached during the mark phase.
 * Returns the total bytes freed. */
uint64_t sweep(uint64_t currentMark){
    uint64_t freed=0;
    Block* b = (Block*) heap;
    Block* end = (Block*)( heap + HEAP_SIZE );
    while( b < end ){
        if( b->prev == b ){
            // this block is not free
            if( b->mark != currentMark ){
                // it was not reached in our mark phase
                freed += b->size;
                memory_free(b);
            }
        }
        char* c = (char*)b;
        c += b->size;
        b = (Block*) c;
    }
    return freed;
}

/* markAndSweep - runs a full mark-and-sweep garbage collection cycle. */
void markAndSweep(uint64_t* rsp, uint64_t* rbp){
    static uint64_t currentMark=1;
    currentMark++;
    uint64_t allocated = markGlobals(currentMark, &dataStart, &dataEnd);
    allocated += markGlobals(currentMark, &bssStart, &bssEnd);
    allocated += markLocals(currentMark, rsp, rbp);
    uint64_t freed = sweep(currentMark);
    debugprints("Garbage collection: ");
    debugprinti(allocated);
    debugprints(" bytes allocated; ");
    debugprinti(freed);
    debugprints(" bytes freed\n");
}

/* memory_alloc - allocates size bytes, triggering GC and retrying on failure. */
static void* memory_alloc(uint64_t* rsp, uint64_t* rbp, uint64_t size)
{
    void* p = do_memory_alloc(size);
    if (!p)
    {
        markAndSweep(rsp, rbp);
        p = do_memory_alloc(size);
        if (!p)
            outOfMemory();
    }
    return p;
}

static void fatal(){
    asm("int3");
}

static void outOfMemory(){
    fatal();
}

/* prepend - inserts block f at the head of the free list. */
static void prepend(Block* f){
    f->next = head;
    f->prev = NULL;
    if(head)
        head->prev = f;
    head = f;
}

/* do_memory_alloc - first-fit allocator. Searches the free list for a block
 * large enough to satisfy size bytes (not including the Block header).
 * Splits the block if the remainder is large enough to hold another Block.
 * Marks the returned block as in-use by setting prev = next = self.
 * Returns a pointer to the usable memory (just past the Block header),
 * or NULL if no suitable block is found. */
static void* do_memory_alloc(uint64_t size){
    uint64_t totalSize = size + sizeof(Block);
    Block* b = head;
    while(b){
        if(b->size >= totalSize){
            uint64_t remainder = b->size - totalSize;
            if(remainder >= sizeof(Block) + 1){
                // split: carve a new free block out of the tail
                char* bc = (char*) b;
                Block* split = (Block*)(bc + totalSize);
                split->size = remainder;
                split->mark = 0;
                insertAfter(split, b);
                b->size = totalSize;
            }
            // remove b from the free list
            if(b->prev)
                b->prev->next = b->next;
            else
                head = b->next;
            if(b->next)
                b->next->prev = b->prev;
            // mark as in-use: prev = next = self
            b->prev = b;
            b->next = b;
            return (void*)(b + 1);
        }
        b = b->next;
    }
    return NULL;
}

/* memory_free - returns block f to the free list in address order, then coalesces. */
void memory_free(Block* f){
    if( f->prev != f )
        fatal();        // already free

    f->prev = NULL;
    f->next = NULL;

    if( !head ){
        head = f;
        return;
    }

    Block* b;

    if( f < head ){
        prepend(f);
        b = head;
    } else {
        b = head;
        while(1){
            if( !b->next ){
                // at end of list; append f to list
                insertAfter(f, b);
                break;
            } else if( b < f && f < b->next ){
                // insert f between b and b->next
                insertAfter(f, b);
                break;
            } else {
                // keep searching
                b = b->next;
            }
        }
    }

    // coalesce adjacent free blocks
    if( b->next && blocksAreContiguous(b, b->next) ){
        combine(b, b->next);
        if( b->next && blocksAreContiguous(b, b->next) )
            combine(b, b->next);
    } else if( b->next ){
        if( b->next->next && blocksAreContiguous(b->next, b->next->next) )
            combine(b->next, b->next->next);
    }
}

void insertAfter(Block* toInsert, Block* afterThis){
    Block* tmp = afterThis->next;
    afterThis->next = toInsert;
    toInsert->prev = afterThis;
    toInsert->next = tmp;
    if(tmp)
        tmp->prev = toInsert;
}

int blocksAreContiguous(Block* a, Block* b){
    char* ac = (char*) a;
    char* bc = (char*) b;
    if( ac + a->size == bc )
        return 1;
    else
        return 0;
}

void combine(Block* a, Block* b){
    a->size += b->size;
    a->next = b->next;
    if( b->next )
        b->next->prev = a;
}

static void debugputc(char c){
    DWORD nw;
    WriteFile(stderr, &c, 1, &nw, 0);
}

static void debugprints(const char* x){
    while(*x){
        debugputc(*x);
        x++;
    }
}

static void debugprinti(int64_t x){
    char buffer[32];
    unsigned count = toDecimal(x, buffer);
    for(unsigned i=0; i<count; ++i)
        debugputc(buffer[i]);
}