using System;
using System.Threading.Tasks;
using static Program.IO;
static class ContextProcess {

    // User Port
    internal static String Gui => BuildGui();
    internal static String Page => BuildPage();

    static String BuildGui() { return App.Get("Gui.js"); }
    static String BuildPage() { return App.Get("Page.js"); }
    static String BuildMain() { return App.Get("Main.js"); }

    internal static String BusPort => $$"""

    """;

    internal static async Task Start() {
        await RunCmdAsync(BuildMain(), "Browser Process Started");
        L("Server Process Started");
        await Task.CompletedTask;
    }
}
