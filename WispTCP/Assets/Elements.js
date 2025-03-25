class CenteredLabel extends HTMLElement{
    static { regElm(this); }
    #text;
    constructor(text) {
        super();
        this.#text = text;
    }
}

class LineLabel extends HTMLElement{
    static { regElm(this); }
    #text;
    constructor(text) {
        super();
        this.#text = text;
    }
}