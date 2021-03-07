using System;
using System.IO;
using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Onnx;

namespace OnnxSharp.Test
{
    [TestClass]
    public class GraphExtensionsTest
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
        public void Info()
        {
            // Arrange
            var model = ModelProto.Parser.ParseFrom(m_createStream);

            // Act
            var actual = model.Graph.Info();

            // Assert
            var expected = ExpectedInfo;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Clean()
        {
            // Arrange
            var model = ModelProto.Parser.ParseFrom(m_createStream);

            // Act
            model.Graph.Clean();

            // Assert
            var graph = model.Graph;
            Assert.AreEqual(1, graph.Input.Count);
            Assert.AreEqual(1, graph.Output.Count);
            var expectedName = $"mnist-8-expected-{nameof(Clean)}.onnx";
            AssertModelBytesEqualToEmbeddedExpected(model, expectedName);
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
            var expectedName = $"mnist-8-expected-{nameof(RemoveInitializersFromInputs)}.onnx";
            AssertModelBytesEqualToEmbeddedExpected(model, expectedName);
        }

        [TestMethod]
        public void RemoveUnnecessaryInitializerReshapes()
        {
            // Arrange
            var model = ModelProto.Parser.ParseFrom(m_createStream);

            // Act
            model.Graph.RemoveUnnecessaryInitializerReshapes();

            // Assert
            var graph = model.Graph;
            Assert.AreEqual(8, graph.Input.Count);
            Assert.AreEqual(1, graph.Output.Count);
            var expectedName = $"mnist-8-expected-{nameof(RemoveUnnecessaryInitializerReshapes)}.onnx";
            AssertModelBytesEqualToEmbeddedExpected(model, expectedName);
        }

        [TestMethod]
        public void SetDim()
        {
            // Arrange
            var model = ModelProto.Parser.ParseFrom(m_createStream);

            // Act
            model.Graph.SetDim(dimIndex: 0, DimParamOrValue.New("N"));

            // Assert
            var graph = model.Graph;
            Assert.AreEqual(9, graph.Input.Count);
            Assert.AreEqual(1, graph.Output.Count);
            var expectedName = $"mnist-8-expected-{nameof(SetDim)}.onnx";
            AssertModelBytesEqualToEmbeddedExpected(model, expectedName);
        }

        static void AssertModelBytesEqualToEmbeddedExpected(ModelProto model, string expectedName)
        {
            var actualBytes = model.ToByteArray();
            //model.WriteToFile(expectedName);
            var expectedBytes = AssemblyResourceLoader.GetBytes(expectedName);
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }

        const string ExpectedInfo = @"## Inputs without Initializer
### Tensors
|Name  |Type      |ElemType|Shape    |SizeInFile|
|:-----|:---------|:-------|--------:|---------:|
|Input3|TensorType|Float   |1x1x28x28|        32|

## Outputs
### Tensors
|Name            |Type      |ElemType|Shape|SizeInFile|
|:---------------|:---------|:-------|----:|---------:|
|Plus214_Output_0|TensorType|Float   | 1x10|        34|

## Inputs with Initializer
### Tensors
|Name                              |Type      |ElemType|Shape    |SizeInFile|
|:---------------------------------|:---------|:-------|--------:|---------:|
|Parameter5                        |TensorType|Float   |  8x1x5x5|        36|
|Parameter6                        |TensorType|Float   |    8x1x1|        32|
|Parameter87                       |TensorType|Float   | 16x8x5x5|        37|
|Parameter88                       |TensorType|Float   |   16x1x1|        33|
|Pooling160_Output_0_reshape0_shape|TensorType|Int64   |        2|        48|
|Parameter193                      |TensorType|Float   |16x4x4x10|        38|
|Parameter193_reshape1_shape       |TensorType|Int64   |        2|        41|
|Parameter194                      |TensorType|Float   |     1x10|        30|

## Initializers (Parameters etc.)
|Name                              |DataType|Dims     |Π(Dims)|[v0,v1..vN] | (Min,Mean,Max)        |SizeInFile|
|:---------------------------------|:-------|--------:|------:|-----------------------------------:|---------:|
|Parameter193                      |Float   |16x4x4x10|   2560|(-7.595E-001,-1.779E-003,1.186E+000)|     10265|
|Parameter87                       |Float   | 16x8x5x5|   3200|(-5.089E-001,-3.028E-002,5.647E-001)|     12824|
|Parameter5                        |Float   |  8x1x5x5|    200|(-9.727E-001,-7.360E-003,1.019E+000)|       823|
|Parameter6                        |Float   |    8x1x1|      8|(-4.338E-001,-1.023E-001,9.164E-002)|        53|
|Parameter88                       |Float   |   16x1x1|     16|(-4.147E-001,-1.554E-001,1.328E-002)|        86|
|Pooling160_Output_0_reshape0_shape|Int64   |        2|      2|                             [1,256]|        46|
|Parameter193_reshape1_shape       |Int64   |        2|      2|                            [256,10]|        39|
|Parameter194                      |Float   |     1x10|     10|(-1.264E-001,-4.777E-006,1.402E-001)|        62|

## Value Infos
### Tensors
|Name                        |Type      |ElemType|Shape     |SizeInFile|
|:---------------------------|:---------|:-------|---------:|---------:|
|Parameter193_reshape1       |TensorType|Float   |    256x10|        40|
|Convolution28_Output_0      |TensorType|Float   | 1x8x28x28|        48|
|Plus30_Output_0             |TensorType|Float   | 1x8x28x28|        41|
|ReLU32_Output_0             |TensorType|Float   | 1x8x28x28|        41|
|Pooling66_Output_0          |TensorType|Float   | 1x8x14x14|        44|
|Convolution110_Output_0     |TensorType|Float   |1x16x14x14|        49|
|Plus112_Output_0            |TensorType|Float   |1x16x14x14|        42|
|ReLU114_Output_0            |TensorType|Float   |1x16x14x14|        42|
|Pooling160_Output_0         |TensorType|Float   |  1x16x4x4|        45|
|Pooling160_Output_0_reshape0|TensorType|Float   |     1x256|        47|
|Times212_Output_0           |TensorType|Float   |      1x10|        35|
";
    }
}
