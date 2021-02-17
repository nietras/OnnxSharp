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
            using var stream = File.OpenWrite(filePath);
            message.WriteTo(stream);
        }
    }
}
