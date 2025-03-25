//::Uses:ElmTooling

class CenteredLabel extends SElement {
    static { regElm(this); }
    #text;
    constructor(text) {
        super();
        this.#text = text;
        this.textContent = this.#text;
    };
};

class LineLabel extends HTMLElement {
    static { regElm(this); }
    #text;
    constructor(text) {
        super();
        this.#text = text;
    };
};

class TextInput extends HTMLElement {
    static {
        const tag = kebab(this.name);
        customElements.define(tag, this);
    }
    Text;

    editStart() { this.contentEditable = true; }
    editEnd() { this.contentEditable = false; }
    constructor() {
        super();
        this.onclick = () => this.editStart();
        this.onblur = () => this.editEnd();
    }
};

class NumberValue extends HTMLElement {
    static {
        const tag = kebab(this.name);
        customElements.define(tag, this);
        elms[this.name] = this;
    };

    value;
    constructor(value) {
        super();
        this.value = Number(value);
        this.innerText = this.value;
        this.onblur = () => { this.value = Number(this.innerText); };
    }

    connectedCallback() {
        this.contentEditable = true;
    }
}

class ArrayValue extends HTMLElement {
    static {
        const tag = kebab(this.name);
        customElements.define(tag, this);
        elms[this.name] = this;
    };

    value = [];

    constructor(value) {
        super();
        this.value = this.parse(value);
        this.innerText = this.format(this.value);

        this.onblur = () => {
            const a_ = this.innerText.substring(1, this.innerText.length - 1);
            this.value = this.parse(a_);
            this.innerText = this.format(this.value);
        };
    }

    parse(v) {
        if (Array.isArray(v)) { return v; }
        else { return v.split(',').map(parseFloat); };
    };

    format(v) { return `[ ${v.join(", ")} ]`; };

    connectedCallback() {
        this.contentEditable = true;
    };
};

class ActionButton extends HTMLElement {
    static {
        const tag = kebab(this.name);
        customElements.define(tag, this);
        elms[this.name] = this;
    };

    label;
    action;

    constructor(label, action) {
        super();
        this.label = label;
        this.action = action;
        this.innerText = this.label;
        this.onclick = this.action;
    };
};



