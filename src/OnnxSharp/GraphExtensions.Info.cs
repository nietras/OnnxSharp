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

            writer.WriteLine("## Inputs");
            Info(graph.Input, writer);

            writer.WriteLine("## Outputs");
            Info(graph.Output, writer);

            writer.WriteLine("## Initializers (Parameters etc.)");
            MarkdownFormatter.Format(graph.Initializer, writer);
        }

        static void Info(IReadOnlyList<ValueInfoProto> valueInfos, TextWriter writer)
        {
            var tensorTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.TensorType).ToList();
            WriteInfoIfAny(tensorTypes, "Tensors", MarkdownFormatter.FormatAsTensors, writer);

            var sequenceTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.SequenceType).ToList();
            WriteInfoIfAny(sequenceTypes, "Sequences", MarkdownFormatter.FormatAsSequences, writer);

            var mapTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.MapType).ToList();
            WriteInfoIfAny(mapTypes, "Maps", MarkdownFormatter.FormatAsMaps, writer);

            var noneTypes = valueInfos.Where(i => i.Type.ValueCase == TypeProto.ValueOneofCase.None).ToList();
            WriteInfoIfAny(noneTypes, "Nones", MarkdownFormatter.FormatAsNones, writer);
        }

        static void WriteInfoIfAny<T>(IReadOnlyList<T> values, string name,
            Action<IReadOnlyList<T>, TextWriter> info, TextWriter writer)
        {
            if (values.Count > 0)
            {
                writer.WriteLine($"### {name}");
                info(values, writer);
                writer.WriteLine();
            }
        }
    }
}
