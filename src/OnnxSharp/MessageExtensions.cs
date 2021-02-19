using Google.Protobuf;
using System.IO;

namespace Onnx
{
    public static partial class MessageExtensions
    {
        /// <summary>
        /// Writes the given <paramref name="message"/> data to the 
        /// given <paramref name="filePath"/> in protobuf encoding.
        /// </summary>
        public static void WriteToFile(this IMessage message, string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Create);
            message.WriteTo(stream);
        }

        /// <summary>
        /// Writes the given <paramref name="message"/> data to the 
        /// given <paramref name="filePath"/> in JSON encoding.
        /// </summary>
        public static void WriteJsonToFile(this IMessage message, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            JsonFormatter.Default.Format(message, writer);
        }
    }
}
