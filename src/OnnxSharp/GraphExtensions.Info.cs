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
            writer.WriteLine();

            writer.WriteLine("## Outputs");
            Info(graph.Output, writer);
            writer.WriteLine();

            writer.WriteLine("## Initializers (Parameters etc.)");
            Info(graph.Initializer, writer);
        }

        static void Info(IReadOnlyList<ValueInfoProto> valueInfos, TextWriter writer)
        {
            var summaries = valueInfos.Select(i => i.Summary()).ToList();
            MarkdownFormatter.Format(summaries, writer);
        }

        static void Info(IReadOnlyList<TensorProto> tensors, TextWriter writer)
        {
            var summaries = tensors.Select(i => i.Summary()).ToList();
            MarkdownFormatter.Format(summaries, writer);
        }
    }
}
