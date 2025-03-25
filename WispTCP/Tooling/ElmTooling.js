//:[ElmTools:

self.regElmOn = (parent, cls, stylettes) => {
    const tag = kebab(cls.name);
    customElements.define(tag, cls);
    parent[cls.name] = cls;
    if (stylettes != null) customRules.define(tag, stylettes);
    return parent[cls.name];
};

self.SElement = class StereotypeElement extends HTMLElement {
    #stype;
    #states = new (class {

    })();
    get Stype() { return this.#stype; }
    set Stype(v) {
        this.#stype = v;
        this.setAttribute('stype', this.#stype);
    }
    stype(v) { this.Stype = v; return this; }
};

//:]ElmTools: