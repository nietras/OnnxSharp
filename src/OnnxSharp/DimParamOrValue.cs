using System;

namespace Onnx
{
    public readonly struct DimParamOrValue
    {
        readonly string _param;
        readonly int _value;

        private DimParamOrValue(string param, int value)
        {
            _param = param;
            _value = value;
        }

        public static DimParamOrValue NewParam(string param)
        {
            if (!IsParamValid(param))
            {
                throw new ArgumentException($"{nameof(param)} must be a non-whitespace string like 'N'.");
            }
            return new DimParamOrValue(param, default);
        }

        public static DimParamOrValue NewValue(int value)
        {
            return new DimParamOrValue(default, value);
        }

        public string Param { get { CheckIsParam(); return _param; } }
        public int Value { get { CheckIsValue(); return _value; } }

        public bool IsParam => !IsValue;
        public bool IsValue => IsParamValid(_param);

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

        static bool IsParamValid(string param) => string.IsNullOrWhiteSpace(param);
    }
}
