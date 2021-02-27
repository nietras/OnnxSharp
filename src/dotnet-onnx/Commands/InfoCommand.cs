using System;
using System.Linq;
using System.Text;
using Google.Protobuf.Collections;
using McMaster.Extensions.CommandLineUtils;
using Onnx;

[Command("info", Description = "Print information about a model e.g. inputs and outputs")]
public class InfoCommand : InputCommand
{
    public InfoCommand(IConsole console) : base(console)
    { }

    protected override void Run(ModelProto model)
    {
        var sb = new StringBuilder();

        _console.WriteLine("Inputs");
        // TODO: Remove initializers from inputs
        Print(model.Graph.Input, t => _console.WriteLine(t));

        _console.WriteLine("## Initializers (Parameters etc.)");
        MarkdownFormatter.Format(model.Graph.Initializer.Select(i => i.Summary()).ToList(), sb);
        _console.WriteLine(sb.ToString());

        _console.WriteLine("Outputs");
        Print(model.Graph.Output, t => _console.WriteLine(t));        
    }

    void Print(RepeatedField<ValueInfoProto> valueInfos, Action<string> log)
    {
        for (int i = 0; i < valueInfos.Count; i++)
        {
            var valueInfo = valueInfos[i];
            log($"Name: {valueInfo.Name} Size in file: {valueInfo.CalculateSize()} {valueInfo.Type} {valueInfo.Type.ValueCase}");
        }
    }

    void Print(RepeatedField<TensorProto> tensors, Action<string> log)
    {
        for (int i = 0; i < tensors.Count; i++)
        {
            var tensor = tensors[i];
            var summary = tensor.Summary();
            
        }
    }
}