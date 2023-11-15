using System;
using System.Collections.Generic;

namespace Stephen.JsonSerializer;

public class JsonSerializerOptions
{
    public IEnumerable<Type> IgnoreTypes { get; } = new List<Type>();
    public bool IgnoreErrors { get; set; }
    public bool Pretty { get; set; } = true;
    public bool IgnoreNulls { get; set; } = false;
}