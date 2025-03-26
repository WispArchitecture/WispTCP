#pragma warning disable CS0164 // This label has not been referenced
using System;
using System.IO;
using System.Threading.Tasks;
using static Program.IO;

static class ContextProcess {
    const String ApplicationRoot = "Application";
    static readonly DirectoryInfo FileRoot = new(Path.Combine(cla.cd, ApplicationRoot));
    static readonly String TaxelStore = Path.Combine(cla.cd, TaxelRoot);

    internal static async Task Init() => await Initialize();
    internal static async Task Start() => await StartProcess();

    //////////////////////////////////////////////////////////////////////////////////////

    interface BOM {
        static String[] TaxelParts = [
            "FormsConfig.js",
            "PersonName.etjs",
            "StreetUsa.etjs",
            "MuniUsa.etjs"
        ];
        static String[] TaxelComponents = [
            "PersonContact.etjs"
        ];
        static String PartsScript => Scripting.Tooling.MergeFiles(TaxelParts, TaxelStore);
        static String ComponentsScript => Scripting.Tooling.MergeFiles(TaxelComponents, FileRoot.FullName);
    }

    static async Task Initialize() {
        ShellAreas:
        await RunCmdAsync(FileRoot.Get("ShellAreas.js"), "Shell Areas Created");
        BomParts:
        await RunCmdAsync(BOM.PartsScript, "Components Parts Registered ");
        await RunCmdAsync(BOM.ComponentsScript, "Components Registered ");
        ShellControls:
        await RunCmdAsync(FileRoot.Get("ShellControls.js"), "Shell Controls Added");
    }

    static async Task StartProcess() {
        ShowContact();
        await Task.CompletedTask;
    }

    static void ShowContact() {
        String script = $$"""
        const contact = new elms.PersonContact({
        Name: { first: "Randy", middle: "D", last: "Buchholz" },
        Street: { number: "512", road: "Maple" },
        Muni: { city: "Austin", county: "Travis", state: "Texas" }
        });

        addTo(Page.Right, contact);
        """;
        RunCmd(script);
    }
    //static String BuildMain() {
    //    return FileRoot.Get("Main.js");
    //}

    //internal static async Task ProcessStart() {
    //    //L("Server Process Started");
    //    //await RunCmdAsync(BuildMain(), "Browser Process Started");
    //    //L("/* Starting Key Processes */");
    //    //await KeyProcesses();
    //    //L("/* Starting Messaging */");
    //    //await Messaging();
    //}

    static async Task KeyProcesses() {
        //L("Performing Process Steps...");
        //L(" - New Membership Started");
        //await Join.Start();
        //await EnrollmentProcess.Start();
        //L(" - Contact Info Collected");
        //await Interests.Start();
        //L(" - Intrests Recorded");
        //await Join.Finish();
        //L(" - New Membership Complete");
        await Task.CompletedTask;
        L(" ...Process Steps Executed Successfully");
    }

    static async Task Messaging() {
        L("Sending Messages...");
        L(" - Change Requests Sent");
        L(" - Flow Transits Sent");
        L(" ...Messages Sent");
        await Task.CompletedTask;
    }
}
