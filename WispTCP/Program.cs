#pragma warning disable CS0164 // This label has not been referenced

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;
using static Program.IO;
using static Settings;
using static System.Text.Encoding;

interface Context {
    internal static String CKey = "";
    internal static String Script => $$"""
        self.Context = class {
            static ckey = '{{CKey}}';
        };
        """;
}

static class Program {
    static Boolean post_ = MessageBus.POST;
    // Node
    static readonly TcpListener Listener;

    static readonly WebSocket cmdsocket;
    static readonly WebSocket msgsocket;

    // Transients
    static readonly NetworkStream stream;
    static readonly String request;

    // User Port
    internal interface IO {
        internal String RunCmd(String cmd) => cmdsocket.TripCmd(cmd);
        internal Task<String> RunCmdAsync(String cmd) => Program.RunCmdAsync(cmd);

        internal static Boolean RunAppFile(out String result, String file) {
            result = cmdsocket.TripCmd(App.Get(file));
            return !result.StartsWith(FailDefault);
        }

        internal static Boolean RunAssetFile(out String result, String file) {
            result = cmdsocket.TripCmd(Assets.Get(file));
            return !result.StartsWith(FailDefault);
        }
    }

    static Program() {
        if (!post_) throw new Exception("POST Failed");
        Environment:
        Listener = new(IP, Port);
        Listener.Start();
        Process.Start(Browser, Url);

        WebBoot:
        GetRequest(out request, out stream);
        stream.Write(UTF8.GetBytes(HttpHeader + KernelScript));
        stream.Close();

        WebBiosConnect:
        GetRequest(out request, out stream);
        stream.Write(AcceptSocket(request, CommandProtocol));
        cmdsocket = WebSocket.CreateFromStream(stream, true, CommandProtocol, TimeSpan.FromMinutes(2));

        SharedContext:
        Send(Context.Script, "Context");

        WebBusConnect:
        Send(BusScript, "Message Socket");
        WebSocket? temp = null; // Favicon order unpredictable
        GetRequest(out request, out stream);
        if (MsgIco(out WebSocket? a_)) temp = a_;
        GetRequest(out request, out stream);
        if (MsgIco(out WebSocket? b_)) temp = b_;
        msgsocket = temp ?? throw new("No Message Socket");
        MessageBus.Bind(msgsocket);

        MountCES:
        Send(BootScript, "BootScript");

        LoadStylex:
        Send(Assets.Get("Stylex.js"), "Stylex");

        LoadCommonElements:
        Send(Assets.Get("Elements.js"), "Elements");

        BusListen:
        Task.Run(static () => {
            while (true) {
                GetRequest(out _, out NetworkStream stream);
                Byte[] headerBytes = UTF8.GetBytes(NotSupportedHeader);
                stream.Write(headerBytes);
                stream.Close();
            }
        });

        RequestListen:
        Task.Run(static () => {
            while (true) {
                GetRequest(out _, out NetworkStream stream);
                Byte[] headerBytes = UTF8.GetBytes(NotSupportedHeader);
                stream.Write(headerBytes);
                stream.Close();
            }
        });

        /* **********   Locals   ********** */

        static void Send(String cmd, String log) {
            var a_ = cmdsocket.TripCmd(cmd);
            var b_ = a_ == SuccessDefault ? log : a_;
            Debug.WriteLine(b_);
        }

        static Boolean MsgIco(out WebSocket? socket) {
            if (request[5..].StartsWith("favicon.ico")) {
                Byte[] icoContent = File.ReadAllBytes(Path.Combine(Assets.FullName, "favicon.ico"));
                String responseHeader = IcoHeader(icoContent);
                Byte[] headerBytes = UTF8.GetBytes(responseHeader);
                stream.Write(headerBytes);
                stream.Write(icoContent);
                stream.Close();
                socket = null;
                return false;
            }
            stream.Write(AcceptSocket(request, MessageProtocol));
            socket = WebSocket.CreateFromStream(stream, true, MessageProtocol, TimeSpan.FromMinutes(2));
            return true;
        }

        static void GetRequest(out String request, out NetworkStream stream) {
            TcpClient client = Listener.AcceptTcpClient();
            stream = client.GetStream();
            Byte[] buffer = new Byte[1024];
            Int32 bytesRead = stream.Read(buffer, 0, buffer.Length);
            request = UTF8.GetString(buffer, 0, bytesRead);
        }

        static Byte[] AcceptSocket(String request, String protocol) {
            var a_ = $"HTTP/1.1 101 Switching Protocols\r\n" +
                    $"Upgrade: websocket\r\n" +
                    $"Connection: Upgrade\r\n" +
                    $"Sec-WebSocket-Accept: {request.WSKey().WSAccept()}\r\n" +
                    $"Sec-WebSocket-Protocol: {protocol}\r\n\r\n";
            return UTF8.GetBytes(a_);
        }

    }

    static async Task Main() {
        MessageBus.Config();
        MessageBus.Enable();
        String rslt;
        Debug.WriteLine(RunAppFile(out rslt, "Shell.js") ? "Shell" : rslt);
        Debug.WriteLine(RunAppFile(out rslt, "Home.js") ? "Home" : rslt);
        await Task.Delay(-1);
    }

    /* **********   Work   ********** */
    static async Task<String> RunCmdAsync(String cmd) {
        TaskCompletionSource<String> tcs = new();
        await Task.Run(() => {
            try {
                String result = cmdsocket.TripCmd(cmd);
                if (result.StartsWith("fail", StringComparison.OrdinalIgnoreCase)) {
                    tcs.SetException(new InvalidOperationException($"Task failed: {result}"));
                } else { tcs.SetResult(result); }
            } catch (Exception ex) { tcs.SetException(ex); }
        });

        return await tcs.Task;
    }
}


partial interface Settings {
    const String CommandProtocol = "transit.command";
    const String MessageProtocol = "transit.message";
    const String SuccessDefault = "success";
    const String FailDefault = "fail";
    const String AssetRoot = "Assets";
    const String AppRoot = "Application";

    const String Title = "Wisp";

    //commandLineArgs: "127.0.0.1 5150 E:\Min\Min.exe"
    static (String cd, String url, Int32 port, String browser) cla = ParseArgs();
    static String Browser = cla.browser;
    static IPAddress IP = IPAddress.Parse(cla.url);
    static Int32 Port = cla.port;
    static String Url = $"http://{cla.url}:{Port}/";
    static DirectoryInfo Assets = new(Path.Combine(cla.cd, AssetRoot));
    static DirectoryInfo App = new(Path.Combine(cla.cd, AppRoot));

    /* **********   Locals   ********** */
    static (String cd, String url, Int32 port, String browser) ParseArgs() {
        String[] cla = Environment.GetCommandLineArgs();
        String fileRoot = cla[0];
        return (
        fileRoot[..(fileRoot.AsSpan().IndexOf("\\bin\\") + 1)],
        cla[1],
        Int32.Parse(cla[2]),
        cla[3]
        );
    }
}

static class FileSysExtensions {
    internal static String Get(this DirectoryInfo self, String file) =>
        File.ReadAllText(Path.Combine(self.FullName, file));
}

partial interface Settings {
    // Browser Scripts
    static String KernelScript = $$"""
        <html><head><title>{{Title}}</title>
        <script>
        self.AsyncFunction = Object.getPrototypeOf(async function () { }).constructor;
        self.cmd = new WebSocket(location.origin.replace('http', 'ws'), '{{CommandProtocol}}');
        cmd.onmessage = async (msg) => {
            try {
            const result = await (new this.AsyncFunction(msg.data))();
            const response = result ?? '{{SuccessDefault}}';
            cmd.send(response);
            }
            catch (e) {
               cmd.send(`{{FailDefault}}: ${e.stack}`);
               console.log(e.stack);
            }
        };
        </script></head></html>
        """;

    static String BusScript = $$"""
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

    static String BootScript = $$$"""
        self.kebab = (str) => str.replace(/([a-z])([A-Z])/g, '$1-$2').replace('_','-').toLowerCase();
        self.flat = (v) => Array.isArray(v) ? v.join('') : v;
        self.crunch = (v) => v.replace(/\s\s+/g, ' ').trim();
        self.customRules = new CSSStyleSheet();
        document.adoptedStyleSheets.push(customRules);
        customRules.define = (tag, styles) => { customRules.insertRule(`${tag} {${flat(styles)}}`); };
        self.elms = class { };
        self.regElm = (cls, stylettes) => {
            const tag = kebab(cls.name);
            customElements.define(tag, cls);
            elms[cls.name] = cls;
            if(stylettes != null) customRules.define(tag, stylettes);
            return elms[cls.name];
        };
        self.addToBody = (e) => document.body.appendChild(e);
        self.addManyToBody = (eg) => eg.forEach(e => addToBody(e));
        self.addTo = (p, e) => p.appendChild(e);
        self.addManyTo = (p, eg) => eg.forEach(e => addTo(p, e));
        self.clearContent = (elm) => { while (elm.firstChild) { elm.removeChild(elm.firstChild); } };
        return 'CES Mounted';
        """;

    const String HttpHeader =
    "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n";

    const String NotSupportedHeader =
    "HTTP/1.1 501 Not Implemented\r\nContent-Type: text/plain\r\n\r\nThe requested feature is not implemented.";

    static String IcoHeader(Byte[] ico)
    => $"HTTP/1.1 200 OK\r\nContent-Type: image/x-icon\r\nContent-Length: {ico.Length}\r\n\r\n";
}
