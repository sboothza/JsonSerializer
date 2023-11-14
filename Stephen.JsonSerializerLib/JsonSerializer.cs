using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stephen.JsonSerializer;

public static class JsonSerializer
{
    public static string Serialize(object source, JsonSerializerOptions options = null)
    {
        var sb = new StringBuilder(1024);
        using (var writer = new LayoutStreamWriter(sb))
        {
            options ??= new JsonSerializerOptions { IgnoreErrors = false, Pretty = false };
            Serialize(source, writer, options);
            return sb.ToString();
        }
    }

    private static void Serialize(object source, LayoutStreamWriter writer, JsonSerializerOptions options)
    {
        if (options.Pretty)
            writer.Indent();
        if (source is null)
        {
            writer.Write("null");
            if (options.Pretty)
                writer.UnIndent();
            return;
        }

        if (options.IgnoreTypes.Any(t => t.IsInstanceOfType(source)))
            return;

        if (source.Flatten(out var result))
        {
            writer.Write($"{result}");
            return;
        }

        //complex
        if (source is IEnumerable sourceEnumerable)
        {
            if (sourceEnumerable is IDictionary dictionarySource)
            {
                //handle dictionary
                writer.Write("{");
                if (options.Pretty)
                    writer.WriteLine();

                writer.Indent();
                dictionarySource.GetEnumerator()
                    .Cast<DictionaryEntry>()
                    .Where(p => !options.IgnoreTypes.Any(t => t.IsInstanceOfType(p.Value)))
                    .DelimitToWriter((entry, wr) =>
                    {
                        wr.Write($"\"{entry.Key}\":");
                        Serialize(entry.Value, wr, options);
                    }, writer, ",");

                writer.Write("}");
                return;
            }

            //handle list
            if (options.Pretty)
                writer.WriteLine();
            writer.Write("[");
            if (options.Pretty)
            {
                writer.WriteLine();
                writer.Indent();
            }
            
            sourceEnumerable.Cast<object>()
                .Where(i => !options.IgnoreTypes.Any(t => t.IsInstanceOfType(i)))
                .DelimitToWriter((entry, wr) =>
                {
                    Serialize(entry, wr, options);
                    wr.WriteLine();
                }, writer, ",");

            if (options.Pretty)
            {
                writer.WriteLine();
                writer.UnIndent();
            }

            writer.Write("]");
            if (options.Pretty)
                writer.UnIndent();
            return;
        }

        //single object
        writer.Write("{");
        if (options.Pretty)
            writer.WriteLine();
        source.GetType()
            .GetProperties()
            .Select(prop => PropertyTuple.Create(options, source, prop.Name))
            .Where(p => !options.IgnoreTypes.Any(t => t.IsInstanceOfType(p.Value)))
            .Where(p => p != null)
            .DelimitToWriter((item, wr) =>
            {
                wr.Write($"\"{item.Name}\":");
                Serialize(item.Value, writer, options);
            }, writer, ",");

        if (options.Pretty)
            writer.WriteLine();
        writer.Write("}");
        if (options.Pretty)
            writer.UnIndent();
    }

    public static T Deserialize<T>(string json)
    {
        var stream = new CharStream(json);
        return Deserialize<T>(stream);
    }

    /// <summary>
    /// Always as follows:
    /// ObjOrList = Object | List
    /// Object = '{' [ Property [',' Property]] '}'  | '"' value '"'  | 'null' | 'true' | 'false' | Number  
    /// List = '[' [ Object [',' Object] ] ']'
    /// Property = '"' Name '"' : ObjOrList
    /// </summary>
    /// <param name="reader"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static T Deserialize<T>(CharStream stream)
    {
        return (T)Deserialize(stream, typeof(T));
    }

    private static object Deserialize(CharStream stream, Type type)
    {
        var obj = ReadObjOrList(stream, type);
        return obj;
    }

    private static object ChangeType(string value, Type type)
    {
        if (type.IsEnum)
            return Enum.Parse(type, value, true);
        return Convert.ChangeType(value, type);
    }

    private static object ReadObjOrList(CharStream stream, Type type)
    {
        char ch = stream.ReadChar();
        while (char.IsWhiteSpace(ch))
            ch = stream.ReadChar();

        switch (ch)
        {
            case '[':
                return ReadList(stream, type);
            case '{':
                return ReadComplexObject(stream, type);
            default:
                stream.Seek(-1, SeekOrigin.Current);
                var stringValue = ReadSimpleObject(stream);
                object value = stringValue.ToLower() switch
                {
                    "null" => null,
                    "true" => true,
                    "false" => false,
                    _ => ChangeType(stringValue, type)
                };
                return value;
        }
    }

    private static string ReadSimpleObject(CharStream stream)
    {
        var ch = stream.ReadChar();
        if (ch == '"')
            return ReadString(stream);

        var sb = new StringBuilder(1024);
        sb.Append(ch);
        do
        {
            ch = stream.ReadChar();
            if (ch is ',' or ']' or '}')
            {
                stream.Seek(-1, SeekOrigin.Current);
                return sb.ToString();
            }

            sb.Append(ch);
        } while (!stream.EndOfStream);

        return sb.ToString();
    }

    private static object ReadComplexObject(CharStream stream, Type type)
    {
        var ch = stream.ReadChar();
        var item = Activator.CreateInstance(type);

        while (ch != '}')
        {
            if (ch == ',')
                ch = stream.ReadChar();
            //read name
            var name = ReadString(stream);
            ch = stream.ReadChar();
            if (ch != ':')
                throw new InvalidOperationException("Invalid json");

            var propInfo = type.GetProperty(name);

            //read value
            var value = ReadObjOrList(stream, propInfo.PropertyType);
            propInfo.SetValue(item, value);
            ch = stream.ReadChar();
        }

        return item;
    }

    private static object ReadList(CharStream stream, Type type)
    {
        var list = Activator.CreateInstance(type);
        var method = type.GetMethod("Add");
        var itemType = type.GetGenericArguments()[0];
        char ch = stream.ReadChar();
        while (ch != ']')
        {
            if (char.IsWhiteSpace(ch))
            {
            }
            else if (ch != ',')
            {
                stream.Seek(-1, SeekOrigin.Current);
                var item = ReadObjOrList(stream, itemType);
                method.Invoke(list, new[] { item });
            }

            ch = stream.ReadChar();
        }

        return list;
    }

    private static string ReadString(CharStream stream)
    {
        var value = new StringBuilder(1024);
        var ch = stream.ReadChar();
        while (ch != '"')
        {
            value.Append(ch);
            ch = stream.ReadChar();
        }

        return value.ToString();
    }
}