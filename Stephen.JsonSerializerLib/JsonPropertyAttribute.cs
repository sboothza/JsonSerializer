using System;
using System.Runtime.Serialization;

namespace Stephen.JsonSerializer;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class JsonPropertyAttribute : Attribute
{
    public string Name { get; set; }
    public bool Ignore { get; set; }

    public JsonPropertyAttribute(string name = "", bool ignore = false)
    {
        Name = name;
        Ignore = ignore;
    }
}