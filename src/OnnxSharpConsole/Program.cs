using System;
using System.IO;
using System.Linq;
using Google.Protobuf;

namespace OnnxSharpConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Examples see https://github.com/onnx/models
            var onnxFileName = @"C:\git\fnd\onnxruntimeextensions\build\IHFood.OnnxRuntimeProfiler_AnyCPU_Debug\TR-CntkModel-SV-4.0.0-remove-init.onnx";
            using (var file = File.OpenRead(onnxFileName))
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
                    dims[0].DimValue = -1;
                    //shape.Dim = dims;
                    // TODO: Change shape
                    value.Type.TensorType.Shape = shape;
                }

                //foreach (var input in inputs)
                //{
                //    var shape = input.Type.TensorType.Shape;
                //}
                //var nodes = graph.Node;
                //foreach (var node in nodes)
                //{
                //    var inputNames = node.Input;
                //}
                using (var outputFile = File.Create(onnxFileName + "NewDynamic.onnx"))
                {
                    model.WriteTo(outputFile);
                }

                using (var output = new StreamWriter(onnxFileName + "New.json"))
                {
                    var fmt = new JsonFormatter(JsonFormatter.Settings.Default);
                    fmt.Format(model, output);
                    //var stream = new CodedOutputStream();
                    //model.WriteTo(output);
                }
            }
        }
    }
}
