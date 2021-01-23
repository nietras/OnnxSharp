using System;
using System.IO;

namespace OnnxSharpConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Examples see https://github.com/onnx/models
            var onnxFileName = @"TEST.onnx";
            using (var input = File.OpenRead(onnxFileName))
            {
                var model = Onnx.ModelProto.Parser.ParseFrom(input);
            }
        }
    }
}
