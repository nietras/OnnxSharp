using System;

namespace Onnx.Formatting
{
    internal record ColumnSpec(string Name, Align Align);
    internal record ColumnSpec<T>(string Name, Align Align, Func<T, string> Get) : ColumnSpec(Name, Align);
}

// https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

