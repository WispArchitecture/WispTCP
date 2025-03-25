// Local Stylettes
var buttonBorder = `border:1px solid black;`
var leftBorder = `border-right:1px solid black;`
var lightBack = `background-color:lightgrey;`;
var medBack = `background-color:grey;`;
var buttonBack = `background-color:#DDD;`;

// Part Styles
customRules.stype(elms.CenteredLabel, "titleLabel", [
    `font-size:38px;`
]);

customRules.stype(elms.ActionButton, "laneButton", [
    stylex.Cell.Box, stylex.CenterText, buttonBorder, buttonBack,
    `width:100%;height:30px;`,
    `font-size:22px;`
]);

class PageHead extends HTMLElement {
    static {
        regElm(this, [
            stylex.FlexBox, stylex.CenterContent, medBack, `\
            grid-area:head;
        `]);
    };
    constructor(title) {
        super();
    }
};

class PageLeft extends HTMLElement {
    static {
        regElm(this, [
            stylex.Lane.Box, leftBorder, lightBack, `\
            grid-area: left; padding:8px;
        `]);
    };

    #items = [];
    addItem(elm) { this.#items.push(elm); }

    constructor() {
        super();
    }
};

class PageRight extends HTMLElement {
    static {
        regElm(this, [stylex.FlexBox, `\
        grid-area:right;`
        ]);
    };
    constructor() {
        super();
    }
};

class PageFoot extends HTMLElement {
    static {
        regElm(this, [stylex.FlexBox, medBack, `\
        grid-area:foot;
        `]);
    };
    constructor() {
        super();
    }
};

self.Header = new elms.PageHead();
self.Left = new elms.PageLeft();
self.Right = new elms.PageRight();
self.Footer = new elms.PageFoot();

self.Page = class { };
addManyToBody([Header, Left, Right, Footer]);

var TitleLabel = new elms.CenteredLabel("Page Title").stype("titleLabel");
var ProceedButton = new elms.ActionButton("Proceed", () => { alert("Clicked"); }).stype("laneButton");

addTo(Header, TitleLabel);
addTo(Left, ProceedButton);
