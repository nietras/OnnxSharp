using System.IO;
using System.Linq;
using Google.Protobuf;

// Examples see https://github.com/onnx/models
var onnxInputFilePath = @"mnist-8.onnx";

var onnxInputFileName = Path.GetFileNameWithoutExtension(onnxInputFilePath);
var outputDirectory = Path.GetDirectoryName(onnxInputFilePath);

using (var file = File.OpenRead(onnxInputFilePath))
{
    var model = Onnx.ModelProto.Parser.ParseFrom(file);
    var graph = model.Graph;
    // Shapes are defined in inputs, values and outputs
    var inputs = graph.Input;
    var values = graph.ValueInfo;
    var outputs = graph.Output;

    foreach (var value in inputs.Concat(values).Concat(outputs))
    {
        var shape = value.Type.TensorType.Shape;
        var dims = shape.Dim;
        var dim = dims[0];
        //dim.DimValue = -1;
        dim.ClearValue();
        dim.DimValue = -1; // Or don't set it
        //dim.DimParam = "None"; // Or don't set it, unset dimension means dynamic
    }

    var fileNameSuffix = "-dynamic-leading-dimension";
    var outputFilePathPrefix = Path.Combine(outputDirectory, onnxInputFileName + fileNameSuffix);

    var onnxOutputFilePath = outputFilePathPrefix + ".onnx";
    using (var outputFile = File.Create(onnxOutputFilePath))
    {
        model.WriteTo(outputFile);
    }

    var jsonOnnxOutputFilePath = outputFilePathPrefix + ".json";
    using (var output = new StreamWriter(jsonOnnxOutputFilePath))
    {
        var fmt = new JsonFormatter(JsonFormatter.Settings.Default);
        fmt.Format(model, output);
    }
}
