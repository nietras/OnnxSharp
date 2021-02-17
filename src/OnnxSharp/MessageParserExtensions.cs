using Google.Protobuf;
using System.IO;

namespace Onnx
{
    /// <summary>
    /// Convenience extension methods to MessageParser.
    /// </summary>
    public static partial class MessageParserExtensions
    {
        /// <summary>
        /// Parse <typeparamref name="T"/> from file via <paramref name="filePath"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T ParseFromFile<T>(this MessageParser<T> parser, string filePath)
            where T : IMessage<T>
        {
            using var stream = File.OpenRead(filePath);
            return parser.ParseFrom(stream);
        }
    }
}
