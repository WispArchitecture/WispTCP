customRules.stype = (cls, type, stylettes) => {
    tag = kebab(cls.name);
    customRules.define(`${tag}[stype='${type}']`, stylettes);
};