self.stylex = class Stylex {
    static ZeroTrim = `margin:0; padding:0; border-width:0;`;
    static FlexBox = `display:inline-flex; box-sizing:border-box;`;
    static GridBox = `position:relative;display:grid;box-sizing:border-box;${this.ZeroTrim}`;
    static CenterContent = `align-items:center; justify-content:center;`;
    static CenterSelf = `align-self:center; justify-self:center;`;
    static CenterText = `align-content:center; text-align:center;`;

    static Fixed = class {
        static Box = Stylex.FlexBox + `position:fixed;`;
        //
        static Row = this.Box + `flex-direction:row;`;
        static RowCentered = this.Box + `flex-direction:row;` + Stylex.CenterContent;
        static Column = this.Box + `flex-direction:column;`;
        static ColumnCentered = this.Box + `flex-direction:column;` + Stylex.CenterContent;
    };

    static Line = class {
        static Box = Stylex.FlexBox + `position:relative;flex-direction:row;`;
        static FlexJoint = this.Box + `flex:1;`;
        static FlexWeight = (weight) => this.Box + `flex:0 0 ${weight};`;
        static FlexFixed = (sizepx) => this.Box + `flex:0 0 ${sizepx}px;`;
    };

    static Lane = class {
        static Box = Stylex.FlexBox + `position:relative;flex-direction:column;`;
        static FlexJoint = this.Box + `flex:1;`;
        static FlexWeight = (weight) => this.Box + `flex:0 0 ${weight};`;
        static FlexFixed = (sizepx) => this.Box + `flex:0 0 ${sizepx}px;`;
    };

    static Cell = class {
        static Box = `position:relative;display:inline-block;`
    };
};