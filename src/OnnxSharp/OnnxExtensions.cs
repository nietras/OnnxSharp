using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onnx
{
    public static partial class OnnxExtensions
    {
        /// <summary>
        /// Remove initializers from inputs of graph in model.
        /// </summary>
        public static ModelProto RemoveInitializersFromInputs(this ModelProto model)
        {
            RemoveInitializersFromInputs(model.Graph);
            return model;
        }

        /// <summary>
        /// Remove initializers from inputs of graph.
        /// </summary>
        // https://github.com/microsoft/onnxruntime/blob/master/tools/python/remove_initializer_from_input.py
        public static GraphProto RemoveInitializersFromInputs(GraphProto graph)
        {
            var inputs = graph.Input;
            var nameToInput = inputs.ToDictionary(i => i.Name, i => i);

            foreach (var initializer in graph.Initializer)
            {
                if (nameToInput.TryGetValue(initializer.Name, out var input))
                {
                    inputs.Remove(input);
                }
            }

            return graph;
        }
    }
}
