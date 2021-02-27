using McMaster.Extensions.CommandLineUtils;
using Onnx;

[Command("clean", Description = "Clean model for inference e.g. remove initializers from inputs")]
public class CleanCommand : InputOutputCommand
{
    public CleanCommand(IConsole console) : base(console)
    { }

    protected override void Run(ModelProto model)
    {
        model.Graph.Clean();
    }
}