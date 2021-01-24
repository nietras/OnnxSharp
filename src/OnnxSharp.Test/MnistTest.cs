using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onnx;

namespace OnnxSharp.Test
{
    [TestClass]
    public class MnistTest
    {
        [TestMethod]
        public void ParseFrom()
        {
            // Arrange
            using var stream = AssemblyResourceLoader.GetStream("mnist-8.onnx");
            
            // Act
            var model = ModelProto.Parser.ParseFrom(stream);

            // Assert
            var graph = model.Graph;
            // For some reason the input count is 9 here, looks like artifact of the onnx file
            Assert.AreEqual(9, graph.Input.Count);
            Assert.AreEqual(1, graph.Output.Count);
        }
    }
}
