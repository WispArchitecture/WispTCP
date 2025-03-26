customRules.define("html", [`height:100%;`]);

customRules.define("body", [
    stylex.GridBox, `\
    grid-template:
    "head head" 48px
    "left right" auto
    "foot foot" 30px
    / 200px auto;
`]);

