self.bus = new WebSocket(location.origin.replace('http', 'ws') + '/bus', '{{MessageProtocol}}');
bus.onmessage = async (msg) => {
    try {
        const result = await (new this.AsyncFunction(msg.data))();
        const response = result ?? '{{SuccessDefault}}';
        bus.send(response);
    }
    catch (e) {
        bus.send(`{{FailDefault}}: ${e.stack}\n${msg.data}`);
        console.log(e.stack);
    }
};

class MsgFrame {
    constructor(format, ckey, body) {
        this.format = format;
        this.ckey = ckey;
        this.body = body;
    }

    static parseMessage(msg) {
        const format = msg.slice(0, 3);
        const ckey = msg.slice(3, 8);
        const body = msg.slice(8);
        return new MsgFrame(format, ckey, body);
    }

    static CkeyChk = (ckey) => { if (ckey.length != 5) throw new Error("CKeyLength"); };


}


bus.signal = (signal, ckey = "_____") => { MessageFrame.CKeyChk(ckey); bus.send(`sig${ckey}${signal}`); };
bus.data = (data, ckey = "_____") => { MessageFrame.CKeyChk(ckey); bus.send(`dat${ckey}${data}`); };
bus.msg = (msg, ckey = "_____") => { MessageFrame.CKeyChk(ckey); bus.send(`msg${ckey}${msg}`); };

self.BusPortReceiver = async (msg) => {
    const msgframe = MsgFrame.parseMessage(msg);
    console.log(msgframe);
};