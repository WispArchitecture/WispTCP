// Local Stylettes
const cfg = shellSettings;
const titleSkel = stylex.CenterContent;
const titleSkin = `border-bottom:1px solid black;background-color:${Pallette.Dark}`;

const leftSkel = `padding:${cfg.leftPad}px; gap:${cfg.leftGap}px;`;
const leftSkin = `border-right:1px solid black;background-color:${Pallette.Med}`;

const rightSkel = `padding:${cfg.rightPad}px;`;
const rightSkin = `border-right:1px solid black;background-color:${Pallette.Light}`;

const footSkel = `padding:${cfg.footPad}px;`;
const footSkin = `border-top:1px solid black;background-color:${Pallette.Dark}`;


self.Page = class Page {
    static {
        class PageHead extends HTMLElement { };
        class PageLeft extends HTMLElement { };
        class PageRight extends HTMLElement { };
        class PageFoot extends HTMLElement { };

        regElm(PageHead, [`grid-area:head;`, stylex.FlexBox, titleSkel, titleSkin]);
        regElm(PageLeft, [`grid-area:left;`, stylex.Lane.Box, leftSkel, leftSkin]);
        regElm(PageRight, [`grid-area:right;`, stylex.Lane.Box, rightSkel, rightSkin]);
        regElm(PageFoot, [`grid-area:foot;`, stylex.FlexBox, footSkel, footSkin]);
    };

    static Header = new elms.PageHead();
    static Left = new elms.PageLeft();
    static Right = new elms.PageRight();
    static Footer = new elms.PageFoot();

    static {
        addManyToBody([this.Header, this.Left, this.Right, this.Footer]);
    }
};