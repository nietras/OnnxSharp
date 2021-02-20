using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf;
using McMaster.Extensions.CommandLineUtils;
using Onnx;

// https://github.com/natemcmaster/CommandLineUtils
// https://natemcmaster.github.io/CommandLineUtils/docs/advanced/dependency-injection.html
// TODO: Change to builder API
// TODO: Clean up
// TODO: Handle multiple command names etc.

// https://github.com/jonstodle/DotNetSdkHelpers/blob/master/src/DotNetSdkHelpers/Program.cs
[Command("dotnet-onnx", Description = "Inspect and manipulate ONNX files"),
 Subcommand(typeof(Clean)),
 //Subcommand(typeof(List)),
 //Subcommand(typeof(Download))
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

        return app.ExecuteAsync(args);
    }
}

public abstract class Command
{
    public async Task OnExecuteAsync()
    {
        try
        {
            await Run();
        }
        //catch (CliException e)
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(e.Message);
            Console.ResetColor();
            //Environment.Exit(e.ExitCode);
        }
    }

    public abstract Task Run();
}

[Command("clean", Description = "Clean graph for inference e.g. remove initializers from inputs")]
public class Clean : Command
{
    [Argument(0, "input", Description = "Input file path")]
    [Required]
    public string Input { get; }

    [Argument(1, "output", Description = "Output file path")]
    [Required]
    public string Output { get; }

    public override Task Run()
    {
        var model = ModelProto.Parser.ParseFromFile(Input);

        model.Graph.RemoveInitializersFromInputs();

        model.WriteToFile(Output);

        return Task.CompletedTask;
    }
}