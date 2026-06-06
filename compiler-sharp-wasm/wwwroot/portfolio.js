// ── portfolio.js (compiler-sharp-wasm) ────────────────────────────────────────
//
// Two-panel demo:
//   Panel 1 (#codeInput) — editable source code textarea
//   Panel 2 (#asmOut)    — x86-64 AT&T assembly from RunAsmText
// ─────────────────────────────────────────────────────────────────────────────

let _compilerReady = false;

function waitForCompiler(callback, maxWaitMs = 20000, intervalMs = 100) {
    const start = Date.now();
    const check = () => {
        if (window.compiler) {
            _compilerReady = true;
            callback();
        } else if (Date.now() - start < maxWaitMs) {
            setTimeout(check, intervalMs);
        } else {
            console.warn('[portfolio] WASM compiler did not load in time.');
        }
    };
    check();
}

function initScrollAnimations() {
    const obs = new IntersectionObserver(entries => {
        entries.forEach(e => { if (e.isIntersecting) e.target.classList.add('v'); });
    }, { threshold: 0.08 });
    document.querySelectorAll('.fiu').forEach(el => obs.observe(el));
}

async function runDemo() {
    const src    = document.getElementById('codeInput')?.value || '';
    const asmOut = document.getElementById('asmOut');

    if (!_compilerReady) {
        if (asmOut) asmOut.innerHTML = `<div class="empty-state"><div class="ei">⬡</div>Loading WASM runtime…</div>`;
        return;
    }

    if (asmOut) asmOut.innerHTML = `<div class="empty-state"><div class="ei">⬡</div>Compiling…</div>`;

    try {
        const asmText = await window.compiler.invokeMethodAsync('RunAsmText', src);
        if (asmOut) {
            if (asmText.startsWith('error:')) {
                asmOut.innerHTML = `<div class="run-error">${escHtml(asmText)}</div>`;
            } else {
                asmOut.innerHTML = `<div class="asm-out">${colorizeAsm(asmText)}</div>`;
            }
        }
    } catch (err) {
        console.error('[portfolio] RunAsmText failed:', err);
        if (asmOut) asmOut.innerHTML = `<div class="run-error">Compiler invocation failed — see console</div>`;
    }
}

function colorizeAsm(text) {
    return text
        .split(/\r?\n/)   // handle both \n and \r\n
        .map(line => {
            const e = escHtml(line);
            if (/^\s*\/\//.test(line))
                return `<span class="asm-comment">${e}</span>`;
            if (/^\s*\w+:/.test(line))
                return e.replace(/^(\s*)(\w+:)/, '$1<span class="asm-label">$2</span>');
            if (/^\s*\./.test(line))
                return e.replace(/(\.\w+)/, '<span class="asm-directive">$1</span>');
            return e
                .replace(/(\/\/.*$)/, '<span class="asm-comment">$1</span>')
                .replace(/(%\w+)/g, '<span class="asm-reg">$1</span>')
                .replace(/^(\s*)(\w+)(\s)/, '$1<span class="asm-op">$2</span>$3');
        })
        .join('<br>');
}

function escHtml(s) {
    return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function handleSend(e) {
    e.preventDefault();
    const b = e.target.querySelector('button');
    if (!b) return;
    b.textContent = '✓ Sent!';
    setTimeout(() => { b.textContent = '▶ Send'; }, 3000);
}

document.addEventListener('DOMContentLoaded', () => {
    initScrollAnimations();

    waitForCompiler(() => {
        console.info('[portfolio] WASM compiler ready — real C# compiler active');
        window.compiler.invokeMethodAsync('RunAsmText', 'func main(): int { return 42 }')
            .then(r => console.log('[portfolio] smoke test:', r.startsWith('error:') ? r : 'PASS'));

        const asmOut = document.getElementById('asmOut');
        if (asmOut && !asmOut.querySelector('.empty-state')) runDemo();
    });
});
