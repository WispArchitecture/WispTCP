#pragma warning disable CS0164 // This label has not been referenced

global using static Settings;
global using static Tracer;

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

using static HttpHelpers;
using static BootScripts;
using BusConfig = MessageBus.Config;

using static System.Text.Encoding;
// 
interface Context {
    internal static String CKey = "";
    internal static String Script => $$"""
        self.Context = class {
            static ckey = '{{CKey}}';
        };
        """;
}

static class Program {
    static Boolean post = Tracer.POST && MessageBus.POST;
    // Node
    static readonly TcpListener Listener;
    static readonly WebSocket CommandPort;
    static readonly WebSocket MessagePort;

    // Temporals
    static readonly NetworkStream stream_;
    static readonly String request_;

    // User Port
    internal interface IO {
        internal static String RunCmd(String cmd) => CommandPort.TripCmd(cmd);
        internal static Task<String> RunCmdAsync(String cmd, String? log = null) => Program.RunCmdAsync(cmd, log);

        internal static Boolean RunAppFile(out String result, String file, String? log = null) {
            result = CommandPort.TripCmd(App.Get(file));
            if (log != null) L(log);
            return !result.StartsWith(FailDefault);
        }

        internal static Boolean RunAssetFile(out String result, String file, String? log = null) {
            result = CommandPort.TripCmd(Assets.Get(file));
            if (log != null) L(log);
            return !result.StartsWith(FailDefault);
        }
    }

    // Build the static machine (Mecha)
    static Program() {
        if (!post) throw new Exception("POST Failed");
        Environment:
        Listener = new(IP, Port);
        Listener.Start();
        Process.Start(Browser, Url);

        WebBoot:
        GetRequest(out request_, out stream_);
        stream_.Write(UTF8.GetBytes(HttpHeader + POSTScript));
        stream_.Close();

        WebBiosConnect:
        GetRequest(out request_, out stream_);
        L("Browser POSTed");
        stream_.Write(AcceptSocket(request_, CommandProtocol));
        CommandPort = WebSocket.CreateFromStream(stream_, true, CommandProtocol, TimeSpan.FromMinutes(2));
        L("Command Link Established");

        SharedContext:
        Send(Context.Script, "Shared Context Registered");

        WebBusConnect:
        Send(BusConfig.BusSocket, "Message Socket Requested");
        // Favicon order is unpredictable, so this crap...
        WebSocket? temp = null;
        GetRequest(out request_, out stream_);
        if (MsgIco(out WebSocket? a_)) temp = a_;
        GetRequest(out request_, out stream_);
        if (MsgIco(out WebSocket? b_)) temp = b_;
        MessagePort = temp ?? throw new("No Message Socket");
        L("Message Link Established");

        WebBusInit:
        MessageBus.Bind(MessagePort);

        CES:
        Send(KernelScript, "CES Kernel Installed");
        Send(Tooling.Get("SheetTooling.js"), "Sheets Tooling Mounted");
        Send(Tooling.Get("ElmTooling.js"), "Elms Tooling Mounted");

        CommonAssets: // Standard Machine
        Send(Assets.Get("Stylex.js"), "Stylex Registered");
        Send(Assets.Get("Elements.vs.js"), "Common Elements Registered ");

        BomAssets: // Customize Machine
        Send(BOM.TaxelScript, "Taxels Registered ");

        RequestListen:
        Task.Run(static () => {
            L("Network Handler Listening");
            loop:
            GetRequest(out _, out NetworkStream stream);
            Byte[] headerBytes = UTF8.GetBytes(NotSupportedHeader);
            stream.Write(headerBytes);
            stream.Close();
            goto loop;
        });

        /* **********   Locals   ********** */

        static void Send(String cmd, String? log = null) {
            var a_ = CommandPort.TripCmd(cmd);
            if (a_.StartsWith(FailDefault)) throw new Exception(a_);
            String b_ = a_ == SuccessDefault && log != null ? log : a_;
            if (log != null) L(b_);
        }

        static Boolean MsgIco(out WebSocket? socket) {
            if (request_[5..].StartsWith("favicon.ico")) {
                Byte[] icoContent = File.ReadAllBytes(Path.Combine(Assets.FullName, "favicon.ico"));
                String responseHeader = IcoHeader(icoContent);
                Byte[] headerBytes = UTF8.GetBytes(responseHeader);
                stream_.Write(headerBytes);
                stream_.Write(icoContent);
                stream_.Close();
                socket = null;
                return false;
            }
            stream_.Write(AcceptSocket(request_, MessageProtocol));
            socket = WebSocket.CreateFromStream(stream_, true, MessageProtocol, TimeSpan.FromMinutes(2));
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
        L("/* Starting Process Code */");
        BusConfig.ConfigClient(ContextProcess.BusPort);
        BusConfig.Enable();
        MessageBus.Start();
        await RunCmdAsync(ContextProcess.Gui, "Gui Shell Configured");
        await RunCmdAsync(ContextProcess.Page, "Gui Page Built");
        await ContextProcess.Start();
        L("WispNode Execution Complete. Terminating.");
        await Task.Delay(-1);
    }

    /* **********   Work   ********** */
    static async Task<String> RunCmdAsync(String cmd, String? log = null) {
        TaskCompletionSource<String> tcs = new();
        await Task.Run(() => {
            try {
                String result = CommandPort.TripCmd(cmd);
                if (result.StartsWith("fail", StringComparison.OrdinalIgnoreCase)) {
                    tcs.SetException(new InvalidOperationException($"Task failed: {result}"));
                } else {
                    tcs.SetResult(result);
                    String b_ = result == SuccessDefault && log != null ? log : result;
                    if (log != null) L(b_);
                }
            } catch (Exception ex) { tcs.SetException(ex); }
        });

        return await tcs.Task;
    }
}

interface Settings {
    const String CommandProtocol = "transit.command";
    const String MessageProtocol = "transit.message";
    const String SuccessDefault = "success";
    const String FailDefault = "fail";
    const String AssetRoot = "Assets";
    const String TaxelRoot = "Taxels";
    const String ToolRoot = "Tooling";
    const String AppRoot = "Application";

    const String Title = "Wisp";

    //commandLineArgs: "127.0.0.1 5150 E:\Min\Min.exe"
    static (String cd, String url, Int32 port, String browser) cla = ParseArgs();
    static String Browser = cla.browser;
    static IPAddress IP = IPAddress.Parse(cla.url);
    static Int32 Port = cla.port;
    static String Url = $"http://{cla.url}:{Port}/";
    static DirectoryInfo Tooling = new(Path.Combine(cla.cd, ToolRoot));
    static DirectoryInfo Taxels = new(Path.Combine(cla.cd, TaxelRoot));
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

interface BOM {
    static String[] Taxels = [
        "PersonName.etjs",
        "StreetUsa.etjs",
        "MuniUsa.etjs"
    ];
    static String TaxelScript => Scripting.Tooling.MergeFiles(Taxels, Path.Combine(cla.cd, TaxelRoot));
}

interface BootScripts {
    // Browser Scripts
    static String POSTScript = $$"""
        <html><head><title>{{Title}}</title>
        <script>
        {{LanguageExtensions()}}
        self.AsyncFunction = Object.getPrototypeOf(async function () { }).constructor;
        self.cmd = new WebSocket(location.origin.replace('http', 'ws'), '{{CommandProtocol}}');
        cmd.onmessage = async (msg) => {
            try {
            const result = await (new this.AsyncFunction(msg.data))();
            const response = result ?? '{{SuccessDefault}}';
            cmd.send(response);
            }
            catch (e) {
               cmd.send(`{{FailDefault}}: ${e.stack}\n${msg.data}`);
               console.log(e.stack);
            }
        };
        </script></head></html>
        """;

    static String KernelScript = $$$"""
        self.customRules = new CSSStyleSheet();
        document.adoptedStyleSheets.push(customRules);
        customRules.define = (tag, styles) => { customRules.insertRule(`${tag} {${flat(styles)}}`); };
        self.elms = class { };
        self.abs = class { };
        self.regElm = (cls, stylettes) => {
            const tag = kebab(cls.name);
            customElements.define(tag, cls);
            elms[cls.name] = cls;
            if (stylettes != null) customRules.define(tag, stylettes);
            return elms[cls.name];
        };
        self.addToBody = (e) => document.body.appendChild(e);
        self.addManyToBody = (eg) => eg.forEach(e => addToBody(e));
        self.addTo = (p, e) => p.appendChild(e);
        self.addManyTo = (p, eg) => eg.forEach(e => addTo(p, e));
        self.clearContent = (elm) => { while (elm.firstChild) { elm.removeChild(elm.firstChild); } };
        return 'CES Mounted';
        """;

    static String LanguageExtensions() => $$"""
        self.Fault = (i) => { throw new Error(i); };
        self.Required = (i) => { return (i != null) ? i : Fault(`${i} Required`); };
        self.Default = (i, d) => { return (i != null) ? i : d; };
        self.Validate = (i, validationFn, errorMessage = 'Validation failed') => { return validationFn(i) ? i : Fault(errorMessage); };
        self.Exists = (v) => { return v !== null && typeof v !== 'undefined'; };
        self.NotExists = (v) => { return !(v !== null && typeof v !== 'undefined'); };
        self.kebab = (str) => str.replace(/([a-z])([A-Z])/g, '$1-$2').replace('_','-').toLowerCase();
        self.flat = (v) => Array.isArray(v) ? v.join('') : v;
        self.crunch = (v) => v.replace(/\s\s+/g, ' ').trim();
        """;
}

internal interface HttpHelpers {
    internal const String HttpHeader =
    "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n";

    internal const String NotSupportedHeader =
    "HTTP/1.1 501 Not Implemented\r\nContent-Type: text/plain\r\n\r\nThe requested feature is not implemented.";

    internal static String IcoHeader(Byte[] ico)
    => $"HTTP/1.1 200 OK\r\nContent-Type: image/x-icon\r\nContent-Length: {ico.Length}\r\n\r\n";
}