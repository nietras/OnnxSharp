using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OnnxSharp.Test
{
    public static class AssemblyResourceLoader
    {
        public static readonly string ResourceNamespace = typeof(AssemblyResourceLoader).Assembly.GetName().Name;
        public const string ResourceNamePrefix = "";

        public static string[] GetStringArray(string resourceName)
        {
            return GetString(resourceName).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetString(string resourceName)
        {
            using (var stream = GetStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetFullResourceName(string resourceName)
        {
            return ResourceNamePrefix + resourceName;
        }

        public static string FindResourceName(Func<string, bool> filter)
        {
            var names = FindResourceNames(filter);

            if (names.Length == 0)
            {
                throw new ArgumentException("Could not find any resource. The desired file might not have been defined as Embedded Resource.");
            }
            else if (names.Length != 1)
            {
                throw new ArgumentException($"Ambiguous name, cannot identify resource - found {names.Length} possible candidates.");
            }

            return names.Single();
        }

        public static string[] FindResourceNames(Func<string, bool> filter)
        {
            var allResourceNames = Assembly.GetExecutingAssembly()
                                           .GetManifestResourceNames();
            var resources = allResourceNames
                .Where(s => s.StartsWith(ResourceNamePrefix))
                .Select(s => s.Substring(ResourceNamePrefix.Length))
                .ToArray();

            return resources.Where(filter).ToArray();
        }

        /// <summary>
        /// http://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
        /// </summary>
        public static Stream GetStream(string resourceName)
        {
            var fullResourceName = GetFullResourceName(resourceName);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullResourceName);
            if (stream == null)
            {
                throw new ArgumentException($"Could not find resource '{resourceName}'. The desired file might not have been defined as Embedded Resource.");
            }
            return stream;
        }
    }
}
