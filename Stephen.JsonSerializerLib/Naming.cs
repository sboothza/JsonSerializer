using System.Collections.Generic;
using System.Linq;
namespace Stephen.JsonSerializer;

public static class Naming
{
    public static string ConvertName(this string name, NamingOptions namingOptions) =>
        namingOptions switch
        {
            NamingOptions.PascalCase => name.ToPascalCase(),
            NamingOptions.CamelCase => name.ToCamelCase(),
            NamingOptions.SnakeCase => name.ToSnakeCase(),
            _ => name
        };

    private static string[] SplitIntoParts(this string name)
    {
        if (string.IsNullOrEmpty(name))
            return [];
        
        if (name.Contains('_'))
        {
            //split snake
            var tempParts = name.Split("_");
            for (var i = 0; i < tempParts.Length; i++)
            {
                tempParts[i] = tempParts[i]
                    .ToLower();
            }
            return tempParts.Where(p => !string.IsNullOrEmpty(p))
                            .ToArray();
        }
        {
            //assume either pascal or camelcase
            var tempParts = new List<string>();
            var index = 0;
            var tempPart = "";
            while (index < name.Length)
            {
                char chr = name[index];
                if (char.IsUpper(chr))
                {
                    tempParts.Add(tempPart);
                    tempPart = "";
                }
                tempPart += chr;
                index++;
            }
            tempParts.Add(tempPart);
            
            for (var i = 0; i < tempParts.Count; i++)
            {
                tempParts[i] = tempParts[i]
                    .ToLower();
            }
            
            return tempParts.Where(p => !string.IsNullOrEmpty(p))
                            .ToArray();
        }
    }

    public static string ToProperCase(this string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        return ("" + name[0]).ToUpper() + name[1..]
            .ToLower();
    }

    public static string ToPascalCase(this string name)
    {
        var parts = SplitIntoParts(name);
        var result = "";
        foreach (var part in parts)
            result += part.ToProperCase();
        return result;
    }
    
    public static string ToCamelCase(this string name)
    {
        var parts = SplitIntoParts(name);
        if (parts.Length == 0)
            return name;

        var result = parts[0];
        foreach (var part in parts[1..])
            result += part.ToProperCase();
        return result;
    }

    public static string ToSnakeCase(this string name)
    {
        var parts = SplitIntoParts(name);
        return string.Join("_", parts);
    }
}