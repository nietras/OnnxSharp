using Onnx;

// Examples see https://github.com/onnx/models
var onnxInputFilePath = @"mnist-8.onnx";

var model = ModelProto.Parser.ParseFromFile(onnxInputFilePath);

var graph = model.Graph;
// Clean graph e.g. remove initializers from inputs that may prevent constant folding
graph.Clean();
// Set dimension in graph to enable dynamic batch size during inference
graph.SetDim(dimIndex: 0, DimParamOrValue.New("N"));
// Get summarized info about the graph
var info = graph.Info();

System.Console.WriteLine(info);

model.WriteToFile(@"mnist-8-clean-dynamic-batch-size.onnx");