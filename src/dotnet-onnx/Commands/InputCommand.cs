using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Onnx;

public abstract class InputCommand : Command
{
    protected readonly IConsole _console;
    protected Action<string> LogInput;

    public InputCommand(IConsole console)
    {
        _console = console;
        LogInput = t => _console.WriteLine(t);
    }

    [Argument(0, "input", Description = "Input file path")]
    [Required]
    public string Input { get; }

    public override Task Run()
    {
        var model = ModelProto.Parser.ParseFromFile(Input);

        LogInput?.Invoke($"Parsed input file '{Input}' of size {model.CalculateSize()}");

        Run(model);

        return Task.CompletedTask;
    }

    protected abstract void Run(ModelProto model);
}
