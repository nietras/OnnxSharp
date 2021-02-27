using System;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

// https://github.com/natemcmaster/CommandLineUtils
// https://natemcmaster.github.io/CommandLineUtils/docs/advanced/dependency-injection.html
// TODO: Change to builder API
// TODO: Clean up
// TODO: Handle multiple command names etc.

// https://github.com/jonstodle/DotNetSdkHelpers/blob/master/src/DotNetSdkHelpers/Program.cs
[Command("dotnet onnx", Description = "Inspect and manipulate ONNX files"),
 Subcommand(typeof(CleanCommand)),
 Subcommand(typeof(SetDimCommand)),
 Subcommand(typeof(InfoCommand))
]
class Program
{
    static Task<int> Main(string[] args)
    {
        var app = new CommandLineApplication<Program>(
            PhysicalConsole.Singleton,
            Directory.GetCurrentDirectory());

        app.Conventions.UseDefaultConventions();
        app.UsePagerForHelpText = false;

        var result = app.Execute(args);

        return Task.FromResult(result);
    }

    public Task<int> OnExecuteAsync(CommandLineApplication app)
    {
        app.ShowHelp();

        return Task.FromResult<int>(0);
    }
}
