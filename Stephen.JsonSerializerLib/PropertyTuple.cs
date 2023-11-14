using System;
namespace Stephen.JsonSerializer;

internal class PropertyTuple
{
    public string Name { get; }
    public object Value { get; }

    private PropertyTuple(object source, string name)
    {
        Name = name;
        Value = source.GetFieldOrPropertyValue(Name);
    }

    public static PropertyTuple Create(JsonSerializerOptions options, object source, string name)
    {
        try
        {
            return new PropertyTuple(source, name);
        }
        catch (Exception)
        {
            if (options.IgnoreErrors)
                return null;
            throw;
        }
    }
}