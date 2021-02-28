using System;

namespace Onnx
{
    /// <summary>
    /// Dimension represented either a string 'Param' or an integer 'Value'.
    /// </summary>
    public readonly struct DimParamOrValue
    {
        readonly string _param;
        readonly int _value;

        private DimParamOrValue(string param, int value)
        {
            _param = param;
            _value = value;
        }

        /// <summary>Create a new named dimension parameter.</summary>
        public static DimParamOrValue New(string param)
        {
            if (!IsParamValid(param))
            {
                throw new ArgumentException($"{nameof(param)} '{param}' must be a non-whitespace string like 'N'.");
            }
            return new DimParamOrValue(param, default);
        }

        /// <summary>Create a new fixed size dimension.</summary>
        public static DimParamOrValue New(int value) =>
            new DimParamOrValue(default, value);

        /// <summary>Get dimension as a named parameter string.</summary>
        public string Param { get { CheckIsParam(); return _param; } }
        /// <summary>Get dimension as an integer value.</summary>
        public int Value { get { CheckIsValue(); return _value; } }

        /// <summary>Is the dimension a named parameter.</summary>
        public bool IsParam => IsParamValid(_param);
        /// <summary>Is the dimension a fixed sized integer.</summary>
        public bool IsValue => !IsParam;

        /// <summary>Converts the dimension to its equivalent string representation.</summary>
        public override string ToString() => IsParam ? Param : Value.ToString();
        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode() => IsParam ? Param.GetHashCode() : Value.GetHashCode();

        void CheckIsParam()
        {
            if (IsValue) 
            { 
                throw new ArgumentException($"{nameof(DimParamOrValue)} is a value '{_value}' not a param."); 
            }
        }

        void CheckIsValue()
        {
            if (IsParam)
            { 
                throw new ArgumentException($"{nameof(DimParamOrValue)} is a param '{_param}' not a value."); 
            }
        }

        static bool IsParamValid(string param) => !string.IsNullOrWhiteSpace(param);
    }
}
