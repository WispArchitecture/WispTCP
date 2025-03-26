const cfg = shellSettings;

customRules.define("html", [`height:100%;`]);

customRules.define("body", [
    stylex.GridBox, `\
    grid-template:
    "head head" ${cfg.headHeight}px
    "left right" auto
    "foot foot" ${cfg.footHeight}px
    / ${cfg.leftWidth}px auto;
`]);

