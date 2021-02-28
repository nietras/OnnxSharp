using McMaster.Extensions.CommandLineUtils;
using Onnx;

[Command("setdim", Description = "Set dimension of reshapes, inputs and outputs of model e.g. set new dynamic or fixed batch size.")]
public class SetDimCommand : InputOutputCommand
{
    public SetDimCommand(IConsole console) : base(console) 
    { }

    [Option("-i|--index", Description = "Dimension index to set. Default = 0.")]
    public int Index { get; } = 0; // Parametize defaults

    [Option("-d|--dim", Description = "Dimension to set.  Default = N. Use string e.g. 'N' for dynamic batch size or integer e.g. '3' for fixed size")]
    public string Dim { get; } = "N";

    protected override void Run(ModelProto model)
    {
        // Should this not be before loading input? Is the abstract base really that good?

        var dimParamOrValue = int.TryParse(Dim, out var dimValue)
            ? DimParamOrValue.New(dimValue)
            : DimParamOrValue.New(Dim);

        _console.WriteLine($"Setting dimension at {Index} to '{dimParamOrValue}'");

        model.Graph.SetDim(Index, dimParamOrValue);
    }
}