using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Onnx;
using OnnxSharpConsole;

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

// Manually hacked model to remove reshape of fixed weights
//var onnxJsonInputFilePathHACK = @"C:\git\eis\ei4-analysis-models\build\OnnxRuntimeProfiler_AnyCPU_Debug\TR-CntkModel-SV-7.18.0-dynamic-leading-dimension-MANUALHACK.json";
//var modelHACK = ModelProto.Parser.ParseJson(File.ReadAllText(onnxJsonInputFilePathHACK));
//var onnxOutputFilePathHACK = @"C:\git\eis\ei4-analysis-models\build\OnnxRuntimeProfiler_AnyCPU_Debug\TR-CntkModel-SV-7.18.0-MANUALHACK.onnx";
//modelHACK.WriteToFile(onnxOutputFilePathHACK);

// Examples see https://github.com/onnx/models
//var onnxInputFilePath = @"mnist-8.onnx";
var onnxInputFilePath = @"C:\git\eis\ei4-analysis-models\build\OnnxRuntimeProfiler_AnyCPU_Debug\TR-CntkModel-SV-7.18.0.onnx";

var onnxInputFileName = Path.GetFileNameWithoutExtension(onnxInputFilePath);
var outputDirectory = Path.GetDirectoryName(onnxInputFilePath);

var model = ModelProto.Parser.ParseFromFile(onnxInputFilePath);
log($"Parsed file '{onnxInputFilePath}' of size {model.CalculateSize()}");

// Save unmodified
var unmodifiedjsonOnnxOutputFilePath = Path.Combine(outputDirectory, onnxInputFileName) + ".json";
model.WriteJsonToFile(unmodifiedjsonOnnxOutputFilePath);

model.RemoveInitializersFromInputs();
model.RemoveUnnecessaryInitializerReshapes();

var graph = model.Graph;

Brainstorm.SetDim(graph);


var fileNameSuffix = "-dynamic-leading-dimension";
var outputFilePathPrefix = Path.Combine(outputDirectory, onnxInputFileName + fileNameSuffix);

var onnxOutputFilePath = outputFilePathPrefix + ".onnx";
model.WriteToFile(onnxOutputFilePath);

var jsonOnnxOutputFilePath = outputFilePathPrefix + ".json";
model.WriteJsonToFile(jsonOnnxOutputFilePath);