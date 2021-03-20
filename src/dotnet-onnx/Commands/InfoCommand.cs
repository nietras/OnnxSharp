using McMaster.Extensions.CommandLineUtils;
using Onnx;

[Command("info", Description = "Print information about a model e.g. inputs and outputs")]
public class InfoCommand : InputCommand
{
    public InfoCommand(IConsole console) : base(console)
    {
        LogInput = null;
    }

    protected override void Run(ModelProto model)
    {
        var writer = _console.Out;
        
        writer.WriteLine($"# {Input}");
        writer.WriteLine();

        model.Graph.Info(writer);
    }
}