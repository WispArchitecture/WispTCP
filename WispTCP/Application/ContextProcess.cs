#pragma warning disable CS0164 // This label has not been referenced
using System;
using System.IO;
using System.Threading.Tasks;
using static Program.IO;

static class ContextProcess {
    // User Port
    internal static async Task Init() => await Initialize();
    internal static async Task Start() => await ProcessStart();
    static DirectoryInfo App = new(Path.Combine(cla.cd, AppRoot));


    static async Task Initialize() {
        await BuildPage();
        BomAssets: // Customize Machine
        await RunCmdAsync(BOM.TaxelScript, "Taxels Registered ");
    }

    static async Task BuildPage() {
        await RunCmdAsync(App.Get("Page.js"), "Gui Shell Configured");
        await RunCmdAsync(App.Get("Page.js"), "Gui Shell Configured");
        await RunCmdAsync(App.Get("KeyProcesses\\PersonContact.etjs"), "Contact Taxel Registeres");
    }

    static String BuildMain() {
        return App.Get("Main.js");
    }

    interface DataBus {
        internal static String ClientShape() => $$"""
        console.log('DataBus Configured');
        """;
    }

    internal static String BusPort => $$"""
    {{DataBus.ClientShape()}}
    """;

    internal static async Task ProcessStart() {
        L("Server Process Started");
        await RunCmdAsync(BuildMain(), "Browser Process Started");
        L("/* Starting Key Processes */");
        await KeyProcesses();
        L("/* Starting Messaging */");
        await Messaging();
    }

    static async Task KeyProcesses() {
        L("Performing Process Steps...");
        L(" - New Membership Started");
        await Join.Start();
        await EnrollmentProcess.Start();
        L(" - Contact Info Collected");
        await Interests.Start();
        L(" - Intrests Recorded");
        await Join.Finish();
        L(" - New Membership Complete");
        L(" ...Process Steps Executed Successfully");
    }

    static async Task Messaging() {
        L("Sending Messages...");
        L(" - Change Requests Sent");
        L(" - Flow Transits Sent");
        L(" ...Messages Sent");
        await Task.CompletedTask;
    }

    interface BOM {
        static String[] Taxels = [
            "PersonName.etjs",
        "StreetUsa.etjs",
        "MuniUsa.etjs"
        ];
        static String TaxelScript => Scripting.Tooling.MergeFiles(Taxels, Path.Combine(cla.cd, TaxelRoot));
    }
}
