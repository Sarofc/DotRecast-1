using System.IO;
using System.Threading;
using DotRecast.Core;
using DotRecast.Recast.Demo;
using DotRecast.Recast.Demo.Logging.Sinks;
using Serilog;

Thread.CurrentThread.Name ??= "main";

InitializeWorkingDirectory();
InitializeLogger();
StartDemo();

static void InitializeLogger()
{
    var format = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} [{ThreadName}:{ThreadId}]{NewLine}{Exception}";
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithThreadId()
        .Enrich.WithThreadName()
        .WriteTo.Async(c => c.LogMessageBroker(outputTemplate: format))
        .WriteTo.Async(c => c.Console())
        .CreateLogger();
}

static void InitializeWorkingDirectory()
{
    var path = RcDirectory.SearchDirectory("resources/dungeon.obj");
    if (!string.IsNullOrEmpty(path))
    {
        var workingDirectory = Path.GetDirectoryName(path) ?? string.Empty;
        workingDirectory = Path.GetFullPath(workingDirectory);
        Directory.SetCurrentDirectory(workingDirectory);
    }
}

static void StartDemo()
{
    var demo = new RecastDemo();
    demo.Run();
}