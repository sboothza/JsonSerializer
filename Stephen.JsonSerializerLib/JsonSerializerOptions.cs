using System;
using System.Collections.Generic;
namespace Stephen.JsonSerializer;

public class JsonSerializerOptions
{
    public List<Type> IgnoreTypes { get; } = new List<Type>();
    public bool IgnoreErrors { get; set; }
}