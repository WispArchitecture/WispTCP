//////////////////////////////////////////////////////////////////////////////
// This file is for designing the kernel script templates.
// Commonly these are copied into C# string literals in the Startup Object.
// The {{rep}} is the replacers for $$""" """ literals.
// The //:[name:, //:]name: Blocks support automation.
// The //:[xxx:$$ indicates the brace nesting level of the literal.
//////////////////////////////////////////////////////////////////////////////

//:[POSTScript:$$
{{ LanguageExtensions() }}
self.AsyncFunction = Object.getPrototypeOf(async function () { }).constructor;
self.cmd = new WebSocket(location.origin.replace('http', 'ws'), '{{CommandProtocol}}');
cmd.onmessage = async (msg) => {
    try {
        const result = await (new this.AsyncFunction(msg.data))();
        const response = result ?? '{{SuccessDefault}}';
        cmd.send(response);
    }
    catch (e) {
        cmd.send(`{{FailDefault}}: ${e.stack}`);
        console.log(e.stack);
    }
};
//:]POSTScript:

/* ********  Custom Elms and Sheets (CES)  ******** */
//:[KernelScript:$$$
self.customRules = new CSSStyleSheet();
document.adoptedStyleSheets.push(customRules);
customRules.define = (tag, styles) => { customRules.insertRule(`${tag} {${flat(styles)}}`); };
self.cls = class { };
self.elms = class { };
self.abs = class { };
self.regElm = (cls, stylettes) => {
    const tag = kebab(cls.name);
    customElements.define(tag, cls);
    elms[cls.name] = cls;
    if (stylettes != null) customRules.define(tag, stylettes);
    return elms[cls.name];
};
self.addToBody = (e) => document.body.appendChild(e);
self.addManyToBody = (eg) => eg.forEach(e => addToBody(e));
self.addTo = (p, e) => p.appendChild(e);
self.addManyTo = (p, eg) => eg.forEach(e => addTo(p, e));
self.clearContent = (elm) => { while (elm.firstChild) { elm.removeChild(elm.firstChild); } };
return 'CES Mounted';
//:]KernelScript:

/* ********  Language Extensions  ******** */
//:[LanguageEXtensions:
self.Fault = (i) => { throw new Error(i); };
self.Required = (i) => { return (i != null) ? i : Fault(`${i} Required`); };
self.Default = (i, d) => { return (i != null) ? i : d; };
self.Validate = (i, validationFn, errorMessage = 'Validation failed') => { return validationFn(i) ? i : Fault(errorMessage); };
self.Exists = (v) => { return v !== null && typeof v !== 'undefined'; };
self.NotExists = (v) => { return !(v !== null && typeof v !== 'undefined'); };
self.kebab = (str) => str.replace(/([a-z])([A-Z])/g, '$1-$2').replace('_', '-').toLowerCase();
self.flat = (v) => Array.isArray(v) ? v.join('') : v;
self.crunch = (v) => v.replace(/\s\s+/g, ' ').trim();
//:]LanguageEXtensions: