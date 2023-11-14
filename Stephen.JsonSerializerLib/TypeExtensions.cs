using System;
using System.Runtime.CompilerServices;

namespace Stephen.JsonSerializer
{
    public static class TypeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Flatten(this object source, out string value)
        {
            switch (source.GetType())
            {
                case { } boolType when boolType == typeof(bool):
                case { } stringType when stringType == typeof(string):
                    value = $"\"{source}\"";
                    return true;
                case { IsPrimitive: true }:
                case { } decimalType when decimalType == typeof(decimal):
                    value = $"{source}";
                    return true;
                case { IsEnum: true }:
                    value = $"\"{source}\"";
                    return true;
                case { } dtType when dtType == typeof(DateTime):
                    value = $"\"{(DateTime)source:O}\"";
                    return true;
                default:
                    value = null;
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetFieldOrPropertyValue(this object source, string name)
        {
            var prop = source.GetType()
                             .GetProperty(name);

            if (prop != null)
                return prop.GetValue(source, null);

            var field = source.GetType()
                              .GetField(name);

            return field?.GetValue(source);
        }
    }
}