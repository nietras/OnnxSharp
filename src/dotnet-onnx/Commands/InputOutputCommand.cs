using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Onnx;

public abstract class InputOutputCommand : Command
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

        Run(model);

        model.WriteToFile(Output);

        return Task.CompletedTask;
    }

    protected abstract void Run(ModelProto model);
}
