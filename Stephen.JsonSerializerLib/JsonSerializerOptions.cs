using System.Collections.Generic;
namespace Stephen.JsonSerializer;

public class JsonSerializerOptions
{
    public bool IgnoreErrors { get; set; }
    public bool DontSerializeNulls { get; set; }
    public bool IgnorePropertyAttributes { get; set; }
    public bool IgnoreCaseDeserializing { get; set; }
    public NamingOptions Naming { get; set; } = NamingOptions.PropertyName;

    public Dictionary<string, string> RemapFields { get; } = new Dictionary<string, string>();

    public readonly static JsonSerializerOptions Empty = new JsonSerializerOptions();
}

public enum NamingOptions
{
    PascalCase = 1,
    SnakeCase = 2,
    CamelCase = 3,
    PropertyName = 4
}