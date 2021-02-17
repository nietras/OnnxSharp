using Google.Protobuf;
using System.IO;

namespace Onnx
{
    public static partial class MessageExtensions
    {
        //
        // Summary:
        //     Writes the given message data to the given stream in protobuf encoding.
        //
        // Parameters:
        //   message:
        //     The message to write to the stream.
        //
        //   output:
        //     The stream to write to.
        public static void WriteToFile(this IMessage message, string filePath)
        {
            using var stream = File.OpenWrite(filePath);
            message.WriteTo(stream);
        }
    }
}
