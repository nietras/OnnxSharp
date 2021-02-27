using McMaster.Extensions.CommandLineUtils;
using Onnx;

[Command("setdim", Description = "Set dimension of reshapes, inputs and outputs of model e.g. set new dynamic or fixed batch size.")]
public class SetDimCommand : InputOutputCommand
{
    readonly IConsole _console;

    public SetDimCommand(IConsole console)
    {
        _console = console;
    }

    [Option("-i|--index", Description = "Dimension index to set. Default = 0.")]
    public int Index { get; } = 0; // Parametize defaults

    [Option("-d|--dim", Description = "Dimension to set.  Default = N. Use string e.g. 'N' for dynamic batch size or integer e.g. '3' for fixed size")]
    public string Dim { get; } = "N";

    protected override void Run(ModelProto model)
    {
        var dimParamOrValue = int.TryParse(Dim, out var dimValue)
            ? DimParamOrValue.NewValue(dimValue)
            : DimParamOrValue.NewParam(Dim);

        model.Graph.SetDim(Index, dimParamOrValue);
    }
}