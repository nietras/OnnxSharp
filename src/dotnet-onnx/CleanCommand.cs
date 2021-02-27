﻿using McMaster.Extensions.CommandLineUtils;
using Onnx;

[Command("clean", Description = "Clean graph for inference e.g. remove initializers from inputs")]
public class CleanCommand : InputOutputCommand
{
    protected override void Run(ModelProto model)
    {
        model.Graph.RemoveInitializersFromInputs();
    }
}