using System.IO;
using System.Text.Json;
using Google.Protobuf;
using Onnx;

namespace OnnxSharp.Test
{
    public static class ModelProtoTestExtensions
    {
        public static void WriteIndentedJsonToFile(this ModelProto model, string filePath)
        {
            var jsonText = JsonFormatter.Default.Format(model);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonText);

            var options = new JsonSerializerOptions() { WriteIndented = true };

            var jsonTextPretty = JsonSerializer.Serialize(jsonElement, options);
            File.WriteAllText(filePath, jsonTextPretty);
            // Below does not indent
            //using var stream = File.Open(filePath, FileMode.Create);
            //using var writer = new Utf8JsonWriter(stream);
            //JsonSerializer.Serialize(writer, jsonElement, options);
        }
    }
}
