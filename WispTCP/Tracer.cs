using System;
using System.Collections.Generic;
using System.Diagnostics;

static class Tracer {
    internal static Boolean POST => true;
    static Tracer() {
        L("Tracer Posted");
    }

    static readonly List<(String msg, String source)> logCache = [];

    // Log to Cache
    internal static void Log(String msg, String source = "") {
        logCache.Add((msg, source));
    }

    // Write Cached Logs
    internal static void WriteCache() {
        foreach ((String msg, String source) in logCache) {
            Debug.WriteLine($"{source}: {msg}");
        }
        logCache.Clear();
    }

    // Write Immediate
    internal static void L(String msg) => Debug.WriteLine(msg);
}