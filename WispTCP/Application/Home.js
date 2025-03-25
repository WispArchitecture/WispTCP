class PageHeader extends HTMLElement {
    static { regElm(this); };
    #titleText = document.title;
    constructor(title) {
        super();
        this.textContent = this.#titleText;
    }
};

class PageContent extends HTMLElement {
    static { regElm(this); };
};

customRules.define("page-header", [
    `position:relative:display:inline-flex;`,
    `flex:0 0 48px;`,
    `font-size: 40px; align-items:center; justify-content:center;`,
    `background-color:grey;`
]);

customRules.define("page-content", [`background-color:lightgrey;`]);


self.Header = new PageHeader();
self.Content = new PageContent();

addManyToBody([Header, Content]);