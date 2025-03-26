// Menu Controls
const cfg = shellSettings;
const laneButtonSkel = `width:100%;height:${cfg.leftBtnHeight}px;font-size:${cfg.leftBtnFontSz}px;` + stylex.CenterText;
const laneActionButtonSkin = `border:1px solid black; background-color:${Pallette.Light};`;
const laneViewButtonSkin = `border:1px solid black; background-color:${Pallette.Med};`;

customRules.stype(elms.CenteredLabel, "titleLabel", [`font-size:${cfg.headFontSz}px;`]);
customRules.stype(elms.ActionButton, "laneAction", [stylex.Cell.Box, laneButtonSkel, laneActionButtonSkin]);
customRules.stype(elms.ActionButton, "laneStep", [stylex.Cell.Box, laneButtonSkel, laneViewButtonSkin]);

const TitleLabel = new elms.CenteredLabel("Page Title").stype("titleLabel");

const About = new elms.ActionButton("About", () => Page.ShowText("About")).stype("laneAction");
const Start = new elms.ActionButton("Start Enrollment", () => Page.ShowView("Start")).stype("laneStep");
const Contact = new elms.ActionButton("Contact Info", () => Page.ShowView("Contact")).stype("laneStep");
const Interest = new elms.ActionButton("Interests", () => Page.ShowView("Interest")).stype("laneStep");
const Finalize = new elms.ActionButton("Finalize", () => Page.ShowView("Finalize")).stype("laneStep");

addTo(Page.Header, TitleLabel);
addManyTo(Page.Left, [About, Start, Contact, Interest, Finalize]);

