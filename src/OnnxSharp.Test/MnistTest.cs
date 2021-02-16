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
            // 9 inputs since includes initializers
            Assert.AreEqual(9, graph.Input.Count);
            Assert.AreEqual(1, graph.Output.Count);
        }

        [TestMethod]
        public void RemoveInitializersFromInputs()
        {
            // Arrange
            using var stream = AssemblyResourceLoader.GetStream("mnist-8.onnx");
            var model = ModelProto.Parser.ParseFrom(stream);

            // Act
            model.RemoveInitializersFromInputs();

            // Assert
            var graph = model.Graph;
            Assert.AreEqual(1, graph.Input.Count);
            Assert.AreEqual(1, graph.Output.Count);
        }
    }
}
