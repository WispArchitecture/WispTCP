using System.Threading.Tasks;

static class Join {
    internal static async Task Start() {
        await Task.CompletedTask;
    }
    internal static async Task Finish() {
        await Task.CompletedTask;
    }
}