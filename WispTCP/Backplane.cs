using System;
using System.Net.WebSockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using MsgFrame = (System.String format, System.String ckey, System.String body);

static class Backplane {
    internal static Boolean POST => true;
    static Backplane() {
        L("Backplane Posted");
    }

    static WebSocket? ws;
    static WebSocket socket => ws ?? throw new("No Socket");

    interface Format {
        internal const String Message = "msg";
        internal const String Data = "dat";
        internal const String Signal = "sig";
    }

    // Message Bus Channels
    static readonly Channel<MsgFrame> messageReceiveBuffer = Channel.CreateUnbounded<MsgFrame>();
    static readonly Channel<MsgFrame> messageSendBuffer = Channel.CreateUnbounded<MsgFrame>();

    // Message Bus Ports
    internal static ChannelWriter<MsgFrame> BusWriter => messageSendBuffer.Writer;
    internal static ChannelWriter<MsgFrame> BusReader => messageSendBuffer.Writer;

    internal interface IO {
        const String CkeyEmpty = "_____";
        internal static void WriteMessage(MsgFrame msg) { messageSendBuffer.Writer.WriteAsync(msg); }
        internal static void WriteData(String data, String ckey = CkeyEmpty) { socket.Send($"{Format.Data}${ckey}${data}"); }
        internal static void SendSignal(String signal, String ckey = CkeyEmpty) { socket.Send($"{Format.Signal}${ckey}${signal}"); }
    }

    internal interface Config {

        internal static void Bind(WebSocket socket) {
            ws = socket;
            L("Message Bus Constructed");
        }

        internal static String MessageSocketCmd = $$"""
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
            """;

        internal static Boolean ConfigClient() {
            String installBusReceiver = $$"""
            self.MsgFrame = class MsgFrame {
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
            }
            self.BusPortReceiver = async (msg) => {
                const msgframe = MsgFrame.parseMessage(msg.data);
                console.log(msgframe);
            };
            """;
            socket.TripCmd(installBusReceiver);
            socket.TripCmd("console.log('Bus Configured')");
            L("Client BusPort Configured");
            return true;
        }

        internal static void Enable() {
            socket.TripCmd($$"""
                self.bus.onmessage = BusPortReceiver;
                """);

            L("Client BusPort Operating");
            // Tests
            //Program.IO.RunCmd("bus.sendSignal('signal');");
            //Program.IO.RunCmd("bus.sendData('data');");
            //Program.IO.RunCmd("bus.sendMessage('message');");
        }
    }

    internal static void Run() {
        Task.Run(static async () => {
            L("Server BusPort Operating");
            loop:
            socket.WaitOnText(out String msgrec);
            var msg = ParseMessage(msgrec);
            switch (msg.format) {
            case Format.Signal: RouteSignal(msg); break;
            case Format.Data: RecordData(msg); break;
            case Format.Message:
                await messageReceiveBuffer.Writer.WriteAsync(msg);
                break;
            default: throw new Exception("Unsupported Message Format");
            }
            goto loop;
        });

        Task.Run(async () => {
            while (await messageSendBuffer.Reader.WaitToReadAsync()) {
                while (messageSendBuffer.Reader.TryRead(out var msg)) {
                    socket.Send($"{Format.Message}${msg.ckey}${msg.body}");
                }
            }
        });

        static MsgFrame ParseMessage(String msg) {
            var format = msg[..3];
            var ckey = msg[3..8];
            var body = msg[8..];
            return (format, ckey, body);
        }

        static async void RouteSignal(MsgFrame msg) {
            L($"Signal Received: {msg.ckey}  {msg.body}");
            await Task.CompletedTask;
        }
        static async void RecordData(MsgFrame msg) {
            L($"Data Received: {msg.ckey}  {msg.body}");
            await Task.CompletedTask;
        }
    }
}
