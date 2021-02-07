using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf;
using McMaster.Extensions.CommandLineUtils;
using Onnx;

// https://github.com/jonstodle/DotNetSdkHelpers/blob/master/src/DotNetSdkHelpers/Program.cs
[Command("dotnet-onnx", Description = "Inspect and manipulate ONNX files"),
 Subcommand(typeof(RemoveInitializersFromInputs)),
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

    //public Task<int> OnExecuteAsync(CommandLineApplication app)
    //{
    //    //var output = CaptureOutput("dotnet", "--version");
    //    //Console.WriteLine(output?.Trim() ?? "Unable to fetch current SDK version");
    //    return Task.FromResult(0);
    //}
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

[Command("removeinitializersfrominputs", Description = "Remove initializers from inputs in graph")]
public class RemoveInitializersFromInputs : Command
{
    [Argument(0, "input", Description = "Input file path")]
    [Required]
    public string Input { get; }

    [Argument(1, "output", Description = "Output file path")]
    [Required]
    public string Output { get; }

    public override Task Run()
    {
        using (var inputFile = File.OpenRead(Input))
        {
            var model = ModelProto.Parser.ParseFrom(inputFile);

            model.RemoveInitializersFromInputs();

            using (var outputFile = File.Create(Output))
            {
                model.WriteTo(outputFile);
            }
        }

        return Task.CompletedTask;
    }
}