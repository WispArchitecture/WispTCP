//:[Create
self.bus = new WebSocket(location.origin.replace('http', 'ws') + '/bus', '{{MessageProtocol}}');
bus.onopen = () => {
    bus.CKeyEmpty = '_____';
    bus.CKeyChk = (ckey) => { if (ckey.length != 5) throw new Error("CKeyLength"); };
    bus.sendSignal = (signal, ckey = bus.CKeyEmpty) => { bus.CKeyChk(ckey); bus.send(`sig${ckey}${signal}`); };
    bus.sendData = (data, ckey = bus.CKeyEmpty) => { bus.CKeyChk(ckey); bus.send(`dat${ckey}${data}`); };
    bus.sendMessage = (msg, ckey = bus.CKeyEmpty) => { bus.CKeyChk(ckey); bus.send(`msg${ckey}${msg}`); };
};
bus.onmessage = async (msg) => {
    try {
        const result = await (new this.AsyncFunction(msg.data))();
        const response = result ?? '{{SuccessDefault}}';
        bus.send(response);
    }
    catch (e) {
        bus.send(`{{FailDefault}}: ${e.stack}\n${msg.data}`);
        console.log(e.stack);
    };
};
//:]Create

//:[Config
self.BusPortReceiver = async (msg) => {
    const msgframe = cls.MsgFrame.parseMessage(msg.data);
    console.log(msgframe);
};
//:]Config

//:[Enable:
self.bus.onmessage = BusPortReceiver;
//:]Enable: