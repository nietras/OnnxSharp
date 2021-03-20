using System;
using System.IO;
using Google.Protobuf;

namespace Onnx
{
    /// <summary>Convenience <see cref="MessageParser{T}"/> extension methods.</summary>
    public static partial class MessageParserExtensions
    {
        /// <summary>
        /// Parse <typeparamref name="T"/> from file via <paramref name="filePath"/>.
        /// </summary>
        public static T ParseFromFile<T>(this MessageParser<T> parser, string filePath)
            where T : IMessage<T>
        {
            using var stream = File.Open(filePath, FileMode.Open);
            return parser.ParseFrom(stream);
        }

        /// <summary>
        /// Parse <typeparamref name="T"/> from file via <paramref name="createStream"/>
        /// and disposes the created stream after parsing is done.
        /// </summary>
        public static T ParseFrom<T>(this MessageParser<T> parser, Func<Stream> createStream)
            where T : IMessage<T>
        {
            using var stream = createStream();
            return parser.ParseFrom(stream);
        }
    }
}
