// ── portfolio.js (compiler-sharp-wasm) ────────────────────────────────────────
//
// The Blazor WASM runtime registers window.compiler (a DotNetObjectReference
// to CompilerBridge) asynchronously after the page loads. This file:
//
//   1. Initialises scroll animations immediately (no compiler needed).
//   2. Waits for window.compiler to become available.
//   3. On Run: calls the REAL C# compiler via JS interop.
//      Falls back to the JS emulator only if WASM hasn't loaded yet.
//
// JS interop pattern:
//   const result = await window.compiler.invokeMethodAsync("RunTreeBox", src);
//
// All five output modes are exposed by CompilerBridge:
//   RunTokJson   → token stream JSON
//   RunTreeBox   → ASCII box-draw tree  (what the demo shows)
//   RunTreeJson  → expression AST JSON
//   RunDotfile   → Graphviz dot string
//   RunTypeCheck → {"legal": true/false}
// ─────────────────────────────────────────────────────────────────────────────

// ── WASM READINESS ────────────────────────────────────────────────────────────
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
            console.warn('[portfolio] WASM compiler did not load in time — JS fallback remains active.');
        }
    };
    check();
}

// ── SCROLL ANIMATIONS ─────────────────────────────────────────────────────────
function initScrollAnimations() {
    const obs = new IntersectionObserver(entries => {
        entries.forEach(e => { if (e.isIntersecting) e.target.classList.add('v'); });
    }, { threshold: 0.08 });
    document.querySelectorAll('.fiu').forEach(el => obs.observe(el));
}

// ── DEMO RUNNER ───────────────────────────────────────────────────────────────
async function runDemo() {
    const src    = document.getElementById('codeInput')?.value || '';
    const tokOut = document.getElementById('tokOut');
    const astOut = document.getElementById('astOut');

    if (_compilerReady) {
        // ── Real compiler path ────────────────────────────────────────────────
        try {
            if (tokOut) {
                setLoading(tokOut);
                const tokJson = await window.compiler.invokeMethodAsync('RunTokJson', src);
                renderTokensFromJson(tokOut, tokJson);
            }
            if (astOut) {
                setLoading(astOut);
                const treeBox = await window.compiler.invokeMethodAsync('RunTreeBox', src);
                renderTreeBox(astOut, treeBox);
            }
        } catch (err) {
            console.error('[portfolio] Compiler invocation failed:', err);
            if (tokOut) tokOut.innerHTML = `<div class="empty-state" style="color:var(--red)">Compiler error — see console</div>`;
            if (astOut) astOut.innerHTML = `<div class="empty-state" style="color:var(--red)">Compiler error — see console</div>`;
        }
    } else {
        // ── JS fallback (WASM still loading) ─────────────────────────────────
        console.info('[portfolio] WASM not ready — using JS emulator');
        const tokens = tokenizeJS(src);
        const ast    = parseJS([...tokens]);
        if (tokOut) renderTokensFromObjects(tokOut, tokens);
        if (astOut) renderTreeBoxFromAST(astOut, ast);
    }
}

// ── OUTPUT RENDERERS ──────────────────────────────────────────────────────────

function setLoading(el) {
    el.innerHTML = '<div class="empty-state"><div class="ei">⬡</div>Running…</div>';
}

/** Renders token stream from the JSON string returned by RunTokJson */
function renderTokensFromJson(container, tokJson) {
    container.innerHTML = '';
    let tokens;
    try { tokens = JSON.parse(tokJson); } catch { container.textContent = tokJson; return; }
    if (!Array.isArray(tokens)) { container.textContent = tokJson; return; }

    const frag = document.createDocumentFragment();
    for (const t of tokens) {
        const row   = document.createElement('div'); row.className = 'tokrow';
        const badge = document.createElement('span');
        badge.className = `toktype TT-${t.sym || 'unknown'}`;
        badge.textContent = t.sym;
        const val = document.createElement('span'); val.className = 'tokval'; val.textContent = t.lexeme || '';
        const pos = document.createElement('span'); pos.className = 'tokpos'; pos.textContent = `${t.line}:${t.column}`;
        row.append(badge, val, pos);
        frag.appendChild(row);
    }
    container.appendChild(frag);
}

/** Renders the raw tree-box string returned by RunTreeBox */
function renderTreeBox(container, text) {
    container.innerHTML = '';
    const pre = document.createElement('div');
    pre.className = 'treebox-out';
    pre.innerHTML = colorizeTreeBox(text);
    container.appendChild(pre);
}

/** Lightweight syntax highlighter for the plain-text tree-box output */
function colorizeTreeBox(text) {
    const esc = s => s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return esc(text)
        .replace(/\b(\w+Node)\b/g,                          '<span class="tb-node">$1</span>')
        .replace(/(Token\{[^}]*\})/g,                       '<span class="tb-tok">$1</span>')
        .replace(/\b(VoidType|IntType|FloatType|StringConstType)\b/g, '<span class="tb-type">$1</span>')
        .replace(/\bnull\b/g,                               '<span class="tb-null">null</span>')
        .replace(/\b(n\d+)\b/g,                             '<span class="tb-uid">$1</span>')
        .replace(/(List&lt;\w+&gt;[^<\n]*)/g,               '<span class="tb-list">$1</span>')
        .replace(/\b([a-z]\w*) : /g,                        '<span class="tb-key">$1</span> : ');
}

// ── JS FALLBACK LEXER ─────────────────────────────────────────────────────────
// Used only while the WASM runtime is still loading.

const TERMINALS_JS = [
    ['VAR',        /^var\b/],
    ['TYPE',       /^(int|float|string)\b/],
    ['FUNCDEF',    /^func\b/],
    ['RETURN',     /^return\b/],
    ['WHILE',      /^while\b/],
    ['FNUM',       /^((\d*\.\d+)|(\d+([eE][-+]?\d+)))\b/],
    ['NUM',        /^\d+\b/],
    ['IF',         /^if\b/],
    ['ELIF',       /^else if\b/],
    ['ELSE',       /^else\b/],
    ['LBRACE',     /^\{/],
    ['RBRACE',     /^\}/],
    ['LPAREN',     /^\(/],
    ['RPAREN',     /^\)/],
    ['LBRACK',     /^\[/],
    ['RBRACK',     /^\]/],
    ['WHITESPACE', /^[\s\r\n]+(\/\/.*)*/],
    ['POWOP',      /^\*\*/],
    ['MULOP',      /^[*/%]/],
    ['INCOP',      /^\+\+/],
    ['DECOP',      /^--/],
    ['ADDOP',      /^\+/],
    ['SUBOP',      /^-/],
    ['UNRSHIFTOP', /^>>>/],
    ['SHIFTOP',    /^(<<|>>)/],
    ['BITOP',      /^[&|^]/],
    ['BITNOTOP',   /^~/],
    ['BOOLOP',     /^(and|or)\b/],
    ['BOOLNOTOP',  /^not\b/],
    ['BOOLCONST',  /^(true|false)\b/],
    ['EQNEQOP',    /^(==|!=)/],
    ['GLOP',       /^(<=|<|>=|>)/],
    ['ASSIGNOP',   /^=/],
    ['DOTOP',      /^\./],
    ['COLON',      /^:/],
    ['COMMAOP',    /^,/],
    ['STRINGCONST',/^"(\\[nt\\"']|[^"\\])*"/],
    ['ID',         /^\w+/],
];
const EOS = '$$';

function tokenizeJS(src) {
    const tokens = []; let pos = 0, line = 1, col = 0, depth = 0;
    while (pos < src.length) {
        const rest = src.slice(pos); let matched = false;
        for (const [sym, rx] of TERMINALS_JS) {
            const m = rest.match(rx); if (!m) continue;
            const val = m[0];
            if (sym === 'WHITESPACE') {
                let nl = false;
                for (const c of val) { if (c === '\n') { line++; col = 0; nl = true; } else col++; }
                if (nl && depth > 0) tokens.push({ sym: EOS, lexeme: '', line, column: col });
            } else {
                tokens.push({ sym, lexeme: val, line, column: col });
                if (sym === 'LBRACE') depth++;
                if (sym === 'RBRACE') depth = Math.max(0, depth - 1);
                for (const c of val) { if (c === '\n') { line++; col = 0; } else col++; }
            }
            pos += val.length; matched = true; break;
        }
        if (!matched) { tokens.push({ sym: 'unknown', lexeme: src[pos], line, column: col }); col++; pos++; }
    }
    tokens.push({ sym: 'EOF', lexeme: '', line, column: col });
    return tokens;
}

// ── JS FALLBACK PARSER ────────────────────────────────────────────────────────
let _nc = 0;
const mkT = (sym, lex, line, col) => ({ sym, lexeme: lex, line, column: col });

function parseJS(tokens) {
    _nc = 0;
    const toks = tokens.filter(t => t.sym !== 'WHITESPACE');
    let pos = 0;
    const peek = () => toks[pos] || { sym: 'EOF' };
    const eat  = () => toks[pos++] || { sym: 'EOF' };
    const chk  = s  => peek().sym === s;

    function program() {
        const funcs = [];
        while (!chk('EOF')) { if (chk('FUNCDEF')) funcs.push(funcdef()); else eat(); }
        return { kind: 'ProgramNode', funcs };
    }
    function funcdef() {
        eat(); const name = eat();
        if (chk('LPAREN')) {
            eat();
            while (!chk('RPAREN') && !chk('EOF') && !chk('LBRACE') && !chk('COLON')) eat();
            if (chk('RPAREN')) eat();
        }
        let ret = 'VoidType';
        if (chk('COLON')) {
            eat(); const r = eat();
            ret = r.lexeme === 'int' ? 'IntType' : r.lexeme === 'float' ? 'FloatType' : r.lexeme === 'string' ? 'StringConstType' : 'VoidType';
        }
        return { kind: 'FuncdefNode', returnType: ret, name: mkT('ID', name.lexeme, name.line, name.column), stmts: stmtsNode() };
    }
    function stmtsNode() {
        const list = [];
        if (chk('LBRACE')) {
            eat();
            while (chk(EOS)) eat();
            while (!chk('RBRACE') && !chk('EOF')) {
                if (chk(EOS)) { eat(); continue; }
                list.push(stmt());
                while (chk(EOS)) eat();
            }
            if (chk('RBRACE')) eat();
        }
        return { kind: 'StmtsNode', stmts: list };
    }
    function stmt() {
        const t = peek();
        if (t.sym === 'RETURN') {
            const r = eat(); while (chk(EOS)) eat();
            if (chk('RBRACE') || chk('FUNCDEF') || chk('EOF')) return { kind: 'ReturnVoidNode', retToken: mkT('RETURN', 'return', r.line, r.column) };
            return { kind: 'ReturnExprNode', expr: expr(), retToken: mkT('RETURN', 'return', r.line, r.column) };
        }
        if (t.sym === 'IF')   { const c = eat(); return { kind: 'CondNode', clause: mkT('IF',   'if',      c.line, c.column), condition: expr(), stmts: stmtsNode() }; }
        if (t.sym === 'ELIF') { const c = eat(); return { kind: 'CondNode', clause: mkT('ELIF', 'else if', c.line, c.column), condition: expr(), stmts: stmtsNode() }; }
        if (t.sym === 'ELSE') { const c = eat(); return { kind: 'CondNode', clause: mkT('ELSE', 'else',    c.line, c.column), condition: { kind: 'Term', token: mkT('NULL', 'NULL', -1, -1), type: null, unique_id: `n${_nc++}` }, stmts: stmtsNode() }; }
        if (t.sym === 'WHILE'){ const c = eat(); return { kind: 'LoopNode', clause: mkT('WHILE','while',   c.line, c.column), condition: expr(), stmts: stmtsNode() }; }
        return { kind: 'ExprStmt', expr: expr() };
    }
    const PREC = { COMMAOP:10, ASSIGNOP:80, BOOLOP:100, EQNEQOP:120, GLOP:120, BITOP:140, SHIFTOP:160, UNRSHIFTOP:160, ADDOP:180, SUBOP:180, MULOP:200, POWOP:220, DOTOP:500, INCOP:500, DECOP:500 };
    const RA  = new Set(['ASSIGNOP', 'POWOP']);
    const PRE = new Set(['SUBOP', 'BITNOTOP', 'BOOLNOTOP', 'INCOP', 'DECOP']);

    function expr() {
        const buf = []; let d = 0;
        while (true) {
            const t = peek();
            if (t.sym === 'LBRACE' || t.sym === 'LBRACK' || t.sym === 'LPAREN') d++;
            if (t.sym === 'RBRACE' || t.sym === 'RBRACK' || t.sym === 'RPAREN') { if (d === 0) break; d--; }
            if (d === 0 && (t.sym === EOS || t.sym === 'EOF')) break;
            buf.push(eat());
        }
        return buf.length ? bld(buf) : null;
    }
    function bld(ts) {
        if (!ts.length) return null;
        if (ts.length === 1) { const t = ts[0]; return { kind: 'Term', token: mkT(t.sym, t.lexeme, t.line, t.column), type: null, unique_id: `n${_nc++}` }; }
        let mn = Infinity, idx = -1, d = 0, ex = true;
        for (let i = 0; i < ts.length; i++) {
            const s = ts[i].sym;
            if (s === 'LPAREN' || s === 'LBRACK') { d++; ex = false; continue; }
            if (s === 'RPAREN' || s === 'RBRACK') { d--; ex = false; continue; }
            if (d > 0) { ex = false; continue; }
            if (PREC[s] !== undefined && !ex) { if (PREC[s] < mn || (PREC[s] === mn && !RA.has(s))) { mn = PREC[s]; idx = i; } ex = true; } else ex = false;
        }
        if (idx > 0 && idx < ts.length - 1) {
            const op = ts[idx], uid = `n${_nc++}`;
            return { kind: ok(op.sym), left: bld(ts.slice(0, idx)), right: bld(ts.slice(idx + 1)), token: mkT(op.sym, op.lexeme, op.line, op.column), type: null, unique_id: uid };
        }
        const last = ts[ts.length - 1];
        if (last.sym === 'INCOP' || last.sym === 'DECOP') return { kind: last.sym === 'INCOP' ? 'IncNode' : 'DecNode', term: bld(ts.slice(0, -1)), token: mkT(last.sym, last.lexeme, last.line, last.column), type: null, unique_id: `n${_nc++}` };
        const first = ts[0];
        if (PRE.has(first.sym)) { const k = first.sym === 'SUBOP' ? 'NegateNode' : first.sym === 'INCOP' ? 'PreIncNode' : first.sym === 'DECOP' ? 'PreDecNode' : first.sym === 'BITNOTOP' ? 'BitNotNode' : 'BoolNotNode'; return { kind: k, term: bld(ts.slice(1)), token: mkT(first.sym, first.lexeme, first.line, first.column), type: null, unique_id: `n${_nc++}` }; }
        if (first.sym === 'LPAREN' && last.sym === 'RPAREN') return bld(ts.slice(1, -1));
        return { kind: 'Term', token: mkT(ts[0].sym, ts.map(t => t.lexeme).join(''), ts[0].line, ts[0].column), type: null, unique_id: `n${_nc++}` };
    }
    const ok = s => ({ ADDOP:'AddNode', SUBOP:'SubNode', MULOP:'MulNode', POWOP:'PowNode', BITOP:'BitNode', SHIFTOP:'ShiftNode', UNRSHIFTOP:'UnRShiftNode', BOOLOP:'BoolNode', EQNEQOP:'EqNeqNode', GLOP:'GreaterLessNode', ASSIGNOP:'AssignNode', COMMAOP:'CommaNode', DOTOP:'DotNode' })[s] || 'BinaryOp';

    return program();
}

// ── JS FALLBACK RENDERERS ─────────────────────────────────────────────────────

function renderTokensFromObjects(container, tokens) {
    container.innerHTML = '';
    const frag = document.createDocumentFragment();
    for (const t of tokens) {
        if (t.sym === 'WHITESPACE') continue;
        const row   = document.createElement('div'); row.className = 'tokrow';
        const badge = document.createElement('span'); badge.className = `toktype ${t.sym === '$$' ? 'TT-EOS' : 'TT-' + (t.sym || 'unknown')}`; badge.textContent = t.sym === '$$' ? '$$' : t.sym;
        const val   = document.createElement('span'); val.className = 'tokval'; val.textContent = (t.lexeme === '' || t.sym === '$$') ? '' : t.lexeme;
        const p     = document.createElement('span'); p.className = 'tokpos'; p.textContent = `${t.line}:${t.column}`;
        row.append(badge, val, p); frag.appendChild(row);
    }
    container.appendChild(frag);
}

function renderTreeBoxFromAST(container, ast) {
    const div = document.createElement('div'); div.className = 'treebox-out';
    div.innerHTML = buildTreeLines(ast).join('\n');
    container.innerHTML = ''; container.appendChild(div);
}

// Treebox line builders (JS fallback — mirrors real Treedump output)
const hh  = (cls, txt) => `<span class="${cls}">${String(txt).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;')}</span>`;
const ln  = (lines, pfx, last, key, val) => lines.push(pfx + (last ? '\u2514\u2500' : '\u251c\u2500') + hh('tb-key', key) + ' : ' + val);
const tokH = t => t ? hh('tb-tok', `Token{ sym=${t.sym} lexeme=${t.lexeme} line=${t.line} column=${t.column} }`) : hh('tb-null', 'null');

function buildTreeLines(ast) {
    const lines = []; if (!ast) return ['null'];
    lines.push(hh('tb-node', 'ProgramNode'));
    const funcs = ast.funcs || [];
    ln(lines, '', true, 'funcs', hh('tb-list', `List&lt;FuncdefNode&gt; with ${funcs.length} element${funcs.length === 1 ? '' : 's'}`));
    funcs.forEach((fn, fi) => {
        const last = fi === funcs.length - 1, pfx = '  ';
        ln(lines, pfx, last, `[${fi}]`, hh('tb-node', 'FuncdefNode'));
        const ip = pfx + (last ? '  ' : '\u2502 ');
        ln(lines, ip, false, 'returnType', hh('tb-type', fn.returnType || 'VoidType'));
        ln(lines, ip, false, 'name', tokH(fn.name));
        ln(lines, ip, true,  'stmts', hh('tb-node', 'StmtsNode'));
        slLines(lines, ip + '  ', fn.stmts?.stmts || []);
    });
    return lines;
}
function slLines(lines, pfx, stmts) {
    const n = stmts?.length || 0;
    ln(lines, pfx, true, 'stmts', hh('tb-list', `List&lt;StmtNode&gt; with ${n} element${n === 1 ? '' : 's'}`));
    stmts?.forEach((s, i) => stLines(lines, pfx + '  ', i === stmts.length - 1, i, s));
}
function stLines(lines, pfx, last, idx, s) {
    if (!s) return;
    ln(lines, pfx, last, `[${idx}]`, hh('tb-node', s.kind));
    const cp = pfx + (last ? '  ' : '\u2502 ');
    if (s.kind === 'ReturnVoidNode') { ln(lines, cp, true, 'retToken', tokH(s.retToken)); }
    else if (s.kind === 'ReturnExprNode') { ln(lines, cp, false, 'expr', hh('tb-node', s.expr?.kind || 'null')); exLines(lines, cp + '\u2502 ', s.expr); ln(lines, cp, true, 'retToken', tokH(s.retToken)); }
    else if (s.kind === 'CondNode' || s.kind === 'LoopNode') { ln(lines, cp, false, 'clause', tokH(s.clause)); ln(lines, cp, false, 'condition', hh('tb-node', s.condition?.kind || 'null')); exLines(lines, cp + '\u2502 ', s.condition); ln(lines, cp, true, 'stmts', hh('tb-node', 'StmtsNode')); slLines(lines, cp + '  ', s.stmts?.stmts || []); }
    else if (s.kind === 'ExprStmt' && s.expr) { ln(lines, cp, true, 'expr', hh('tb-node', s.expr.kind)); exLines(lines, cp + '  ', s.expr); }
}
function exLines(lines, pfx, node) {
    if (!node) return;
    const isUnary  = ['IncNode','DecNode','PreIncNode','PreDecNode','NegateNode','BitNotNode','BoolNotNode'].includes(node.kind);
    const isBinary = node.left !== undefined;
    if (node.kind === 'Term') { ln(lines, pfx, false, 'token', tokH(node.token)); ln(lines, pfx, false, 'type', hh('tb-null', 'null')); ln(lines, pfx, true, 'unique_id', hh('tb-uid', node.unique_id || 'n?')); }
    else if (isBinary) { ln(lines, pfx, false, 'left', hh('tb-node', node.left?.kind || 'null')); exLines(lines, pfx + '\u2502 ', node.left); ln(lines, pfx, false, 'right', hh('tb-node', node.right?.kind || 'null')); exLines(lines, pfx + '\u2502 ', node.right); ln(lines, pfx, false, 'token', tokH(node.token)); ln(lines, pfx, false, 'type', hh('tb-null', 'null')); ln(lines, pfx, true, 'unique_id', hh('tb-uid', node.unique_id || 'n?')); }
    else if (isUnary) { ln(lines, pfx, false, 'term', hh('tb-node', node.term?.kind || 'null')); exLines(lines, pfx + '\u2502 ', node.term); ln(lines, pfx, false, 'token', tokH(node.token)); ln(lines, pfx, false, 'type', hh('tb-null', 'null')); ln(lines, pfx, true, 'unique_id', hh('tb-uid', node.unique_id || 'n?')); }
}

// ── CONTACT FORM ──────────────────────────────────────────────────────────────
function handleSend(e) {
    e.preventDefault();
    const b = e.target.querySelector('button');
    if (!b) return;
    b.textContent = '✓ Sent!';
    setTimeout(() => { b.textContent = '▶ Send'; }, 3000);
}

// ── INIT ──────────────────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    initScrollAnimations();

    // Start polling for window.compiler. Once WASM loads, re-run the demo
    // automatically so the user sees real output without needing to click again.
    waitForCompiler(() => {
        console.info('[portfolio] WASM compiler ready — real C# compiler active');
        // Only auto-rerun if user has already clicked Run (panels not empty-state)
        const astOut = document.getElementById('astOut');
        if (astOut && !astOut.querySelector('.empty-state')) {
            runDemo();
        }
    });
});
