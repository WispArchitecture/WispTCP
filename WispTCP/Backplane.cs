using System;
using System.Net.WebSockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using MsgFrame = (System.String? ckey, System.String? format, System.String body);

static class MessageBus {
    internal static Boolean POST => true;
    static WebSocket? ws;
    static WebSocket socket => ws ?? throw new("No Socket");

    static readonly Channel<MsgFrame> busread = Channel.CreateUnbounded<MsgFrame>();
    static readonly Channel<MsgFrame> buswrite = Channel.CreateUnbounded<MsgFrame>();

    interface Format {
        internal const String Message = "msg";
        internal const String Data = "dat";
        internal const String SIgnal = "sig";
    }

    static MessageBus() {
        L("Backplane Posted");
    }

    internal static void Bind(WebSocket socket) {
        ws = socket;
        L("Message Bus Constructed");
    }

    internal static void Start() {
        Task.Run(static async () => {
            L("Server BusPort Operating");
            loop:
            socket.WaitOnText(out String msg);
            var a_ = ParseMessage(msg);
            await buswrite.Writer.WriteAsync(a_);
            goto loop;
        });

        static MsgFrame ParseMessage(String msg) {
            return (null, null, msg);
        }
    }

    internal interface IO {
        static ChannelWriter<MsgFrame> BusSend => buswrite.Writer;
        internal static void WriteToBus(MsgFrame v) {
            _ = BusSend.WriteAsync(v);
        }

        internal static ChannelReader<MsgFrame> BusReceive => busread.Reader;
    }

    internal interface Config {
        internal static String BusSocket = $$"""
        self.bus = new WebSocket(location.origin.replace('http', 'ws') + '/bus', '{{MessageProtocol}}');
        bus.onmessage = async (msg) => {
            try {
            const result = await (new this.AsyncFunction(msg.data))();
            const response = result ?? '{{SuccessDefault}}';
            bus.send(response);
            }
            catch (e) {
               bus.send(`{{FailDefault}}: ${e.stack}`);
               console.log(e.stack);
            }
        };
        """;

        internal static Boolean ConfigClient(String busPortScript) {
            var success = socket.TripCmd(busPortScript);
            socket.TripCmd("console.log('Bus Configured')");
            L("Client BusPort Configured");
            return true;
        }

        internal static Boolean Enable() {
            socket.TripCmd($$"""
            self.bus.onmessage = (msg) => { console.log(msg.data); }
            """);
            L("Client BusPort Operating");
            return true;
        }
    }
}
