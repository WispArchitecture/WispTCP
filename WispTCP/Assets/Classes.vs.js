self.cls.MsgFrame = class MsgFrame {
    constructor(format, ckey, body) {
        this.format = format;
        this.ckey = ckey;
        this.body = body;
    };

    static parseMessage(msg) {
        const format = msg.slice(0, 3);
        const ckey = msg.slice(3, 8);
        const body = msg.slice(8);
        return new MsgFrame(format, ckey, body);
    };
};