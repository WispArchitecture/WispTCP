
using System;
using System.Net.WebSockets;
using System.Threading.Channels;

// Singleton Class
static class MessageBus {
    internal static Boolean POST => true;
    static WebSocket? ws;
    static WebSocket socket => ws ?? throw new("No Socket");

    static readonly Channel<(String ckey, String format, String body)> busread = Channel.CreateUnbounded<(String ckey, String format, String body)>();
    static readonly Channel<(String ckey, String format, String body)> buswrite = Channel.CreateUnbounded<(String ckey, String format, String body)>();

    static MessageBus() { }

    internal static void Bind(WebSocket socket) {
        MessageBus.ws = socket;
    }

    internal interface IO {
        static ChannelWriter<(String ckey, String format, String body)> BusSend => buswrite.Writer;
        internal static void WriteToBus((String ckey, String format, String body) v) {
            _ = BusSend.WriteAsync(v);
        }

        internal static ChannelReader<(String ckey, String format, String body)> BusReceive => busread.Reader;
    }

    internal static Boolean Config() {
        socket.TripCmd("console.log('Bus Configured')");
        return true;
    }

    internal static Boolean Enable() {
        socket.TripCmd($$"""
        self.bus.onmessage = (msg) => { console.log(msg.data); }
        """);
        return true;
    }
}
