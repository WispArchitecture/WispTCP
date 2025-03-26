//::Uses:ElmTooling

class CenteredLabel extends SElement {
    static { regElm(this, []); }
    #text;
    constructor(text) {
        super();
        this.#text = text;
        this.textContent = this.#text;
    };
};

class LineLabel extends SElement {
    static { regElm(this); }
    #text;
    constructor(text) {
        super();
        this.#text = text;
    };
};

class TextInput extends SElement {
    static {
        regElm(this)
    }
    #text;
    get Text() { return this.#text; }
    set Text(v) { this.#text = v; this.#show(); }

    #show() { this.textContent = this.#text; }

    #parse() { return this.textContent; };

    #format() { this.#text; }

    #editStart() { this.contentEditable = true; }

    #editEnd() { this.contentEditable = false; }

    constructor(text) {
        super();
        this.#text = text ?? "";
        this.onclick = () => this.#editStart();
        this.onblur = () => this.#editEnd();
        this.#show();
    }
};

class NumberValue extends SElement {
    static {
        regElm(this);
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

class ArrayValue extends SElement {
    static {
        regElm(this);
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

class ActionButton extends SElement {
    static {
        regElm(this);
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
