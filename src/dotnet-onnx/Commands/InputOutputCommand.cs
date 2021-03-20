using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Onnx;

public abstract class InputOutputCommand : Command
{
    protected readonly IConsole _console;

    public InputOutputCommand(IConsole console)
    {
        _console = console;
    }

    [Argument(0, "input", Description = "Input file path")]
    [Required]
    public string Input { get; }

    [Argument(1, "output", Description = "Output file path")]
    [Required]
    public string Output { get; }

    public override Task Run()
    {
        var model = ModelProto.Parser.ParseFromFile(Input);

        _console.WriteLine($"Parsed input file '{Input}' of size {model.CalculateSize()}");

        Run(model);

        model.WriteToFile(Output);

        _console.WriteLine($"Wrote output file '{Output}' of size {model.CalculateSize()}");

        return Task.CompletedTask;
    }

    protected abstract void Run(ModelProto model);
}
