self.Home = class { };

class PageHeader extends HTMLElement {
    static { regElmOn(Home, this); };
    constructor(title) {
        super();
    }
};

class PageContent extends HTMLElement {
    static { regElmOn(Home, this); };
};

customRules.define("page-header", [
    `position:relative:display:inline-flex;`,
    `flex:0 0 48px;`,
    `font-size: 40px; align-items:center; justify-content:center;`,
    `background-color:grey;`
]);

customRules.define("page-content", [`background-color:lightgrey;`]);

customRules.define("centered-label[stype='typed']", [`background-color:green;`]);


self.Header = new PageHeader();
self.Lab = new elms.CenteredLabel("Me").stype("typed");

self.Content = new Home.PageContent();


addTo(Header, Lab);
addManyToBody([Header, Content]);