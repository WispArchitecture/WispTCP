// Local Stylettes
class Pallette {
    static Light = "#DDD";
    static Med = "#999";
    static Dark = "#555";
}

var titleSkel = stylex.CenterContent;
var titleSkin = `border-bottom:1px solid black;background-color:${Pallette.Dark}`;

var leftSkel = `padding:8px; gap:12px;`;
var leftSkin = `border-right:1px solid black;background-color:${Pallette.Med}`;

var rightSkel = `padding:12px;`;
var rightSkin = `border-right:1px solid black;background-color:${Pallette.Light}`;

var footSkel = `padding:8px;`;
var footSkin = `border-top:1px solid black;background-color:${Pallette.Dark}`;

var laneButtonSkel = `width:100%;height:30px;font-size:18px;` + stylex.CenterText;
var laneActionButtonSkin = `border:1px solid black; background-color:${Pallette.Light};`;
var laneViewButtonSkin = `border:1px solid black; background-color:${Pallette.Med};`;

// Stereotype Styles
customRules.stype(elms.CenteredLabel, "titleLabel", [`font-size:38px;`]);
customRules.stype(elms.ActionButton, "laneAction", [stylex.Cell.Box, laneButtonSkel, laneActionButtonSkin]);
customRules.stype(elms.ActionButton, "laneStep", [stylex.Cell.Box, laneButtonSkel, laneViewButtonSkin]);

self.Page = class Page {
    static {
        class PageHead extends HTMLElement {
            static {
                regElm(this, [`grid-area:head;`, stylex.FlexBox,
                    titleSkel, titleSkin]);
            };
        };

        class PageLeft extends HTMLElement {
            static {
                regElm(this, [`grid-area:left;`, stylex.Lane.Box,
                    leftSkel, leftSkin]);
            };
        };

        class PageRight extends HTMLElement {
            static {
                regElm(this, [`grid-area:right;`, stylex.Lane.Box,
                    rightSkel, rightSkin]);
            };
        };

        class PageFoot extends HTMLElement {
            static {
                regElm(this, [`grid-area:foot;`, stylex.FlexBox,
                    footSkel, footSkin]);
            };
        };
    };

    static Header = new elms.PageHead();
    static Left = new elms.PageLeft();
    static Right = new elms.PageRight();
    static Footer = new elms.PageFoot();

    static {
        addManyToBody([this.Header, this.Left, this.Right, this.Footer]);
    }

    static ShowText() {
        clearContent(this.Right);
        this.Right.textContent = `\
        Line 1\n
        Line 2\n
        `;
    };

    static ShowView(view) {
        clearContent(this.Right);
        switch (view) {
            case "Start": this.Right.textContent = "Getting Start View (Not Implemented)"; break;
            case "Contact": this.Right.textContent = "Getting Contact View (Not Implemented)"; break;
            case "Interest": this.Right.textContent = "Getting Interest View (Not Implemented)"; break;
            case "Finalize": this.Right.textContent = "Getting Finalize View (Not Implemented)"; break;
        };
    };
};

var TitleLabel = new elms.CenteredLabel("Page Title").stype("titleLabel");

var About = new elms.ActionButton("About", () => Page.ShowText("About")).stype("laneAction");
var Start = new elms.ActionButton("Start Enrollment", () => Page.ShowView("Start")).stype("laneStep");
var Contact = new elms.ActionButton("Contact Info", () => Page.ShowView("Contact")).stype("laneStep");
var Interest = new elms.ActionButton("Interests", () => Page.ShowView("Interest")).stype("laneStep");
var Finalize = new elms.ActionButton("Finalize", () => Page.ShowView("Finalize")).stype("laneStep");

addTo(Page.Header, TitleLabel);
addManyTo(Page.Left, [About, Start, Contact, Interest, Finalize]);

