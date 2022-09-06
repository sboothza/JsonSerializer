using System;
using System.Linq;
using System.Reflection;

namespace Stephen.JsonSerializer
{
    public static class TypeExtensions
    {
        public static bool Flatten(this object source, out string value)
        {
            switch (source.GetType())
            {
                case { IsPrimitive: true }:
                case { IsEnum: true }:
                case {} decimalType when decimalType == typeof(decimal):
                    value = $"{source}";
                    return true;

                case {} stringType when stringType == typeof(string):
                    value = $"\"{source}\"";
                    return true;

                case {} dtType when dtType == typeof(DateTime):
                    value = $"\"{(DateTime)source:O}\"";
                    return true;

                default:
                    value = null;
                    return false;
            }
        }

        public static bool IsSimple(this Type type)
        {
            return type.IsPrimitive        ||
                   type.IsEnum             ||
                   type == typeof(string)  ||
                   type == typeof(decimal) ||
                   type.IsValueType;
        }

        public static MemberInfo GetFieldOrProperty(this Type type, string name)
        {
            var prop = type.GetProperty(name);

            if (prop != null)
                return prop;

            return type.GetField(name);
        }

        public static object GetFieldOrPropertyValue(this object source, string name)
        {
            try
            {
                var type = source.GetType();
                var prop = type.GetProperty(name);

                if (prop != null)
                    return prop.GetValue(source, null);

                var field = type.GetField(name);

                return field?.GetValue(source);
            }
            catch
            {
                return null;
            }
        }

        public static void SetFieldOrProperty<T>(this Type type, string name, T value)
        {
            var targetProperty = type.GetProperty(name);
            if (targetProperty != null)
                targetProperty.SetValue(type, value, null);

            var targetField = type.GetField(name);
            if (targetField != null)
                targetField.SetValue(type, value);
        }

        public static void DumpProperties(this Type type)
        {
            var props = type.GetProperties()
                            .Select(p => new
                                         {
                                             Name = p.Name,
                                             TypeName = p.PropertyType.Name
                                         })
                            .Union(type.GetFields()
                                       .Select(p => new
                                                    {
                                                        Name = p.Name,
                                                        TypeName = p.FieldType.Name
                                                    }))
                            .ToList();

            foreach (var prop in props)
            {
                Console.WriteLine($"public {prop.TypeName} {prop.Name} {{get; set;}}");
            }
        }
    }
}
