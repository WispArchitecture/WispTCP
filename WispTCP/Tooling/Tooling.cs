using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Threading;
using static System.Text.Encoding;

// Interface Over 
interface Scripting {
    // Config
    //static String rootPath = Assets.FullName;

    // User Port

    /* **********   Work   ********** */
    interface Tooling {
        internal static String MergeFiles(String[] files, String rootPath) {
            String[] buf = new String[files.Length];
            for (Int32 i = 0; i < files.Length; i++) {
                String path = files[i];
                buf[i] = $$"""
            //:[ {{path}}
            {{File.ReadAllText(Path.Combine(rootPath, path))}}
            //:] {{path}}

            """;
            }
            return String.Join("\n", buf);
        }
    }
}

// Interface Over File Contents
interface Sections {
    // Config
    static String rootPath = Assets.FullName;

    // User Port
    internal Dictionary<String, String> FileSections(String fileName) => Tooling.GetSections(fileName);


    /* **********   Work   ********** */
    interface Tooling {
        static Dictionary<String, String> GetSections(String fileName) {
            String[] source = File.ReadAllLines(Path.Combine(rootPath, fileName));
            return ExtractSections(source);
        }

        static Dictionary<String, String> ExtractSections(String[] lines) {
            Dictionary<String, String> sections = [];
            foreach ((String section, Int32 s, Int32 e) in IdentifySections(lines)) {
                sections[section] = String.Join(Environment.NewLine, lines[s..(e + 1)]);
            }
            return sections;
        }

        static List<(String SectionName, Int32 StartLine, Int32 EndLine)> IdentifySections(String[] lines) {
            List<(String, Int32, Int32)> ranges = [];
            for (Int32 i = 0; i < lines.Length; i++) {
                String line = lines[i].Trim();
                if (line.StartsWith("//:[")) {
                    String name = line[4..].Trim();
                    Int32 startLine = i;
                    while (++i < lines.Length && !lines[i].Trim().StartsWith("//:]")) ;
                    if (i < lines.Length && lines[i].Trim()[4..].Trim() == name) {
                        ranges.Add((name, startLine, i));
                    }
                }
            }
            return ranges;
        }
    }
};

static class FileSysExtensions {
    internal static String Get(this DirectoryInfo self, String file) =>
        File.ReadAllText(Path.Combine(self.FullName, file));
}

static class WebSocketExtensions {
    static readonly CancellationToken NC = CancellationToken.None;

    internal static void Send(this WebSocket self, String msg) {
        _ = self.SendAsync(UTF8.GetBytes(msg), 0, true, NC);
    }

    internal static void SendFile(this WebSocket self, String path) {
        var msg = System.IO.File.ReadAllText(path);
        _ = self.SendAsync(UTF8.GetBytes(msg), 0, true, NC);
    }

    internal static void WaitOnText(this WebSocket self, out String msg) {
        ArraySegment<Byte> buf = WebSocket.CreateServerBuffer(1024);
        msg = UTF8.GetString(buf[..(self.ReceiveAsync(buf, NC).Result).Count]);
    }

    internal static void Trip(this WebSocket self, out String result, String script) {
        self.Send(script); self.WaitOnText(out result);
    }

    internal static String TripCmd(this WebSocket self, String cmd) {
        _ = self.SendAsync(UTF8.GetBytes(cmd), 0, true, NC);
        ArraySegment<Byte> buf = WebSocket.CreateServerBuffer(1024);
        return UTF8.GetString(buf[..(self.ReceiveAsync(buf, NC).Result).Count]);
    }

    internal static String WSKey(this String request) {
        const String keyHeader = "Sec-WebSocket-Key: ";
        Int32 startIndex = request.IndexOf(keyHeader) + keyHeader.Length;
        if (startIndex >= keyHeader.Length) {
            Int32 endIndex = request.IndexOf("\r\n", startIndex);
            return request.Substring(startIndex, endIndex - startIndex).Trim();
        }
        throw new Exception("Socket Key Fail");
    }

    internal static String WSAccept(this String secWebSocketKey) {
        const String webSocketGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        String concatenated = secWebSocketKey + webSocketGuid;
        Byte[] hashedBytes = SHA1.HashData(UTF8.GetBytes(concatenated));
        return System.Convert.ToBase64String(hashedBytes);
    }
}