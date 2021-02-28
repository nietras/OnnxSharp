using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Onnx.Formatting;

namespace Onnx
{
    public static partial class GraphExtensions
    {
        /// <summary>Summarize information about the <paramref name="graph"/>.</summary>
        public static string Info(this GraphProto graph)
        {
            var writer = new StringWriter();
            graph.Info(writer);
            return writer.ToString();
        }

        /// <summary>Summarize information about the <paramref name="graph"/>.</summary>
        public static void Info(this GraphProto graph, TextWriter writer)
        {
            var initializerNameSet = new HashSet<string>(graph.Initializer.Select(i => i.Name));
            var inferenceInputs = graph.Input.Where(i => !initializerNameSet.Contains(i.Name)).ToList();

            writer.WriteLine("## Inputs without Initializers");
            Info(inferenceInputs, writer);

            writer.WriteLine("## Outputs");
            Info(graph.Output, writer);

            writer.WriteLine("## Initializers (Parameters etc.)");
            MarkdownFormatter.Format(graph.Initializer, writer);
        }

        static void Info(IReadOnlyList<ValueInfoProto> valueInfos, TextWriter writer)
        {
            var tensorTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.TensorType).ToList();
            if (tensorTypes.Count > 0)
            {
                writer.WriteLine("### Tensors");
                MarkdownFormatter.FormatAsTensors(tensorTypes, writer);
                writer.WriteLine();
            }
            var sequenceTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.SequenceType).ToList();
            if (sequenceTypes.Count > 0)
            {
                writer.WriteLine("### Sequences");
                MarkdownFormatter.FormatAsSequences(sequenceTypes, writer);
                writer.WriteLine();
            }
            var mapTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.MapType).ToList();
            var noneTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.None).ToList();
        }
    }
}
