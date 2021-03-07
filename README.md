![Build and test](https://github.com/nietras/OnnxSharp/workflows/.NET/badge.svg)
[![Stars](https://img.shields.io/github/stars/nietras/OnnxSharp)](https://github.com/nietras/OnnxSharp/stargazers)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

|What        |Links and Status|
|---------------|------|
|`OnnxSharp`  |[![NuGet](https://img.shields.io/nuget/v/OnnxSharp)](https://www.nuget.org/packages/OnnxSharp/) [![Downloads](https://img.shields.io/nuget/dt/OnnxSharp)](https://www.nuget.org/packages/OnnxSharp/) |
|`dotnet-onnx`|[![NuGet](https://img.shields.io/nuget/v/dotnet-onnx)](https://www.nuget.org/packages/dotnet-onnx/) [![Downloads](https://img.shields.io/nuget/dt/dotnet-onnx)](https://www.nuget.org/packages/dotnet-onnx/) |

# `OnnxSharp` library and `dotnet-onnx` tool
ONNX format parsing and manipulation in C# and in command line .NET tool.

# Quick Guide
Install latest version of .NET:
* PowerShell (Windows): [https://dot.net/v1/dotnet-install.ps1](https://dot.net/v1/dotnet-install.ps1)
* Bash (Linux/macOS): [https://dot.net/v1/dotnet-install.sh](https://dot.net/v1/dotnet-install.sh)

|What          |How (code)                                         |How (tool)                     |
|--------------|-------------------------------------------------------|-------------------------------|
|Install       |`dotnet add PROJECT.csproj package OnnxSharp`      |`dotnet tool install dotnet-onnx -g`    |
|Parse         |`var model = ModelProto.Parser.ParseFromFile("mnist-8.onnx");`|-    |
|Info          |`var info = model.Info();`|`dotnet onnx info mnist-8.onnx`    |
|Clean         |`model.Graph.Clean();`  |`dotnet onnx clean mnist-8.onnx mnist-8-clean.onnx`    |
|SetDim        |`model.Graph.SetDim();`  |`dotnet onnx setdim mnist-8.onnx mnist-8-setdim.onnx`    |

# Source Code
Base functionality is based on:
```
.\protoc.exe .\onnx.proto3 --csharp_out=OnnxSharp
```
Everything else written in beautiful C# 9.0 as extensions to this.

# Example Code
```csharp
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
```