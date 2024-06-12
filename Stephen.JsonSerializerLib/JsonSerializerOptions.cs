using System.Collections.Generic;
namespace Stephen.JsonSerializer;

public class JsonSerializerOptions
{
    public bool IgnoreErrors { get; set; }
    public bool IgnoreNulls { get; set; }
    public bool IgnoreAttributes { get; set; }

    public Dictionary<string, string> RemapFields { get; set; } = new Dictionary<string, string>();

    public static JsonSerializerOptions Empty = new JsonSerializerOptions
    {
        RemapFields =
        {
        }
    };
}