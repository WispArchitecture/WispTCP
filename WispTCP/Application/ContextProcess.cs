using System;

static class ContextProcess {

    // User Port
    internal static String Gui => BuildGui();
    internal static String Main => BuildMain();

    static String BuildGui() { return App.Get("Gui.js"); }
    static String BuildMain() { return App.Get("Main.js"); }

    internal static String BusPort => $$"""

    """;
}
