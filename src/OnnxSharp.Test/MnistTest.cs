using System;
using System.IO;
using System.Text.Json;
using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onnx;

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
            model.Graph.RemoveInitializersFromInputs();

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

            //model.WriteJsonToFile("mnist-8.json");

            // Act
            model.Graph.RemoveUnnecessaryInitializerReshapes();

            //model.WriteJsonToFile("mnist-8-after.json");

            //model.WriteToFile("mnist-8-after.onnx");

            // Assert
            var graph = model.Graph;
            //Assert.AreEqual(1, graph.Input.Count);
            //Assert.AreEqual(1, graph.Output.Count);
        }

        [TestMethod]
        public void SetDim()
        {
            // Arrange
            var model = ModelProto.Parser.ParseFrom(m_createStream);

            //model.WriteToFile("mnist-8.onnx");
            //model.WriteIndentedJsonToFile($"mnist-8-{nameof(SetDim)}-before.json");

            // Act
            model.Graph.SetDim(dimIndex: 0, DimParamOrValue.New("N"));

            //model.WriteToFile($"mnist-8-{nameof(SetDim)}-after.onnx");
            //model.WriteIndentedJsonToFile($"mnist-8-{nameof(SetDim)}-after.json");

            // Assert
            var graph = model.Graph;
            //Assert.AreEqual(1, graph.Input.Count);
            //Assert.AreEqual(1, graph.Output.Count);
        }
    }
}
