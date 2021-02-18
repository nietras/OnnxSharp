using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Onnx;

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

// Examples see https://github.com/onnx/models
//var onnxInputFilePath = @"mnist-8.onnx";
var onnxInputFilePath = @"C:\git\eis\ei4-analysis-models\build\OnnxRuntimeProfiler_AnyCPU_Debug\TR-CntkModel-SV-7.18.0.onnx";

var onnxInputFileName = Path.GetFileNameWithoutExtension(onnxInputFilePath);
var outputDirectory = Path.GetDirectoryName(onnxInputFilePath);

var model = ModelProto.Parser.ParseFromFile(onnxInputFilePath);
log($"Parsed file '{onnxInputFilePath}' of size {model.CalculateSize()}");

model.RemoveInitializersFromInputs();

var graph = model.Graph;
// Shapes are defined in inputs, values and outputs
var inputs = graph.Input;
var values = graph.ValueInfo;
var nodes = graph.Node;
var initializers = graph.Initializer;
var outputs = graph.Output;

var allValues = inputs.Concat(values).Concat(outputs).ToArray();

var namesToIgnore = new HashSet<string>();
var reshapes = nodes.Where(n => n.OpType == "Reshape").ToArray();
foreach (var reshape in reshapes)
{
    // Reshape has two inputs (data and shape)
    // Second hence is the shape
    var shapeInputName = reshape.Input[1];
    var shape = initializers.Single(v => v.Name == shapeInputName);
    Debug.Assert(shape.DataType == (int)TensorProto.Types.DataType.Int64);
    var dims = shape.Dims;
    if (dims.Count > 0 && dims[0] > 0)
    {
        // Data may be stored as Int64 or Raw (fixed-width, little-endian)
        var int64Data = shape.Int64Data;
        var rawData = shape.RawData;
        var rawAsInt64Data = MemoryMarshal.Cast<byte, long>(rawData.Span);
        Debug.Assert(rawAsInt64Data.Length == dims[0]);
        if (rawAsInt64Data[0] == 1) // Dimension we replace
        {
            var newShape = rawAsInt64Data.ToArray();
            newShape[0] = -1;
            shape.RawData = ByteString.CopyFrom(MemoryMarshal.Cast<long, byte>(newShape.AsSpan()));
        }
        else
        {
            // FAILS IF ADDING
            //foreach (var output in reshape.Output)
            //{
            //    namesToIgnore.Add(output);
            //}
        }
    }
    // 
    log(reshape.ToString());
}

foreach (var value in allValues)
{
    //var denotation = value.Type.Denotation;
    if (!namesToIgnore.Contains(value.Name))
    {
        var shape = value.Type.TensorType.Shape;
        var dims = shape.Dim;
        var dim = dims[0];
        dim.ClearValue();
        //dim.DimValue = -1; // Or don't set it
        dim.DimParam = "N";
    }
    else
    {
        // FAILS IF WE DO NOT CHANGE OUTPUT OF THE ONE RESHAPE
        // Reshape
        log(value.ToString());
    }
}

//foreach (var node in nodes)
//{
//    var shape = node.Output;
//    var dims = shape.Dim;
//    var dim = dims[0];
//    dim.ClearValue();
//    //dim.DimValue = -1; // Or don't set it
//    dim.DimParam = "N";
//}

var fileNameSuffix = "-dynamic-leading-dimension";
var outputFilePathPrefix = Path.Combine(outputDirectory, onnxInputFileName + fileNameSuffix);

var onnxOutputFilePath = outputFilePathPrefix + ".onnx";
model.WriteToFile(onnxOutputFilePath);

var jsonOnnxOutputFilePath = outputFilePathPrefix + ".json";
model.WriteJsonToFile(jsonOnnxOutputFilePath);