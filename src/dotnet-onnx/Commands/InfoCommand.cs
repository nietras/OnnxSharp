﻿using System;
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
        _console.WriteLine("Inputs");
        Print(model.Graph.Input, t => _console.WriteLine(t));

        _console.WriteLine("Initializers");
        Print(model.Graph.Initializer, t => _console.WriteLine(t));

        _console.WriteLine("Outputs");
        Print(model.Graph.Output, t => _console.WriteLine(t));        
    }

    void Print(RepeatedField<ValueInfoProto> valueInfos, Action<string> log)
    {
        for (int i = 0; i < valueInfos.Count; i++)
        {
            var valueInfo = valueInfos[i];
            log($"Name: {valueInfo.Name} Size in file: {valueInfo.CalculateSize()} {valueInfo.Type}");
        }
    }

    void Print(RepeatedField<TensorProto> tensors, Action<string> log)
    {
        for (int i = 0; i < tensors.Count; i++)
        {
            var tensor = tensors[i];
            log($"Name: {tensor.Name} Size in file: {tensor.CalculateSize()} {tensor.Dims} {tensor.DataType} {tensor.DataLocation} ");
        }
    }
}