![Build and test](https://github.com/nietras/OnnxSharp/workflows/.NET/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/OnnxSharp)](https://www.nuget.org/packages/OnnxSharp/)
[![Downloads](https://img.shields.io/nuget/dt/OnnxSharp)](https://www.nuget.org/packages/OnnxSharp/)
[![Stars](https://img.shields.io/github/stars/nietras/OnnxSharp)](https://github.com/nietras/OnnxSharp/stargazers)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

# OnnxSharp
ONNX format parsing and manipulation in C#.

# Status
Pretty much just:
```
.\protoc.exe .\onnx.proto3 --csharp_out=OnnxSharp
```

# Example Code
```csharp
using System.IO;
using Google.Protobuf;

// Examples see https://github.com/onnx/models
var onnxFilePath = @"mnist-8.onnx";

using var fileStream = File.OpenRead(onnxFilePath);

var model = Onnx.ModelProto.Parser.ParseFrom(fileStream);

var graph = model.Graph;
var inputs = graph.Input;
var valueInfos = graph.ValueInfo;
var outputs = graph.Output;
```