using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onnx;
using System;
using System.IO;

namespace OnnxSharp.Test
{
    [TestClass]
    public class MnistTest
    {
        readonly Func<Stream> m_createStream = () => AssemblyResourceLoader.GetStream("mnist-8.onnx");

        [TestMethod]
        public void ParseFrom()
        {
            // Act
            var model = ModelProto.Parser.ParseFrom(m_createStream);

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
            var model = ModelProto.Parser.ParseFrom(m_createStream);

            // Act
            model.RemoveInitializersFromInputs();

            // Assert
            var graph = model.Graph;
            Assert.AreEqual(1, graph.Input.Count);
            Assert.AreEqual(1, graph.Output.Count);
        }

        [TestMethod]
        public void RemoveUnnecessaryInitializerReshapes()
        {
            // Arrange
            var model = ModelProto.Parser.ParseFrom(m_createStream);

            model.WriteJsonToFile("mnist-8.json");

            // Act
            model.RemoveUnnecessaryInitializerReshapes();

            model.WriteJsonToFile("mnist-8-after.json");

            model.WriteToFile("mnist-8-after.onnx");

            // Assert
            var graph = model.Graph;
            //Assert.AreEqual(1, graph.Input.Count);
            //Assert.AreEqual(1, graph.Output.Count);
        }
    }
}
