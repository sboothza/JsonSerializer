using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stephen.JsonSerializer
{
    public static class JsonSerializer
    {
        public static string Serialize(object source, JsonSerializerOptions options = null)
        {
            var writer = new StringWriter();
            Serialize(source, writer, options);
            return writer.ToString();
        }

        private static void Serialize(object source, TextWriter writer, JsonSerializerOptions options)
        {
            if (source is null)
            {
                writer.Write("null");
                return;
            }

            options ??= new JsonSerializerOptions();

            if (source.Flatten(out var result))
            {
                writer.Write(result);
                return;
            }

            //complex
            if (source is IEnumerable sourceEnumerable)
            {
                if (sourceEnumerable is IDictionary dictionarySource)
                {
                    //handle dictionary
                    writer.Write("{");
                    
                    var dictionaryEntries = dictionarySource.GetEnumerator()
                        .Cast<DictionaryEntry>();

                    dictionaryEntries.ProcessList(entry =>
                    {
                        writer.Write($"\"{entry.Key}\" : ");
                        Serialize(entry.Value, writer, options);

                        writer.Write(",");
                    }, entry =>
                    {
                        writer.Write($"\"{entry.Key}\" : ");
                        Serialize(entry.Value, writer, options);
                    });

                    writer.Write("}");
                    return;
                }

                //handle list
                writer.Write("[");

                var listEntries = sourceEnumerable.Cast<object>();

                listEntries.ProcessList(o =>
                {
                    Serialize(o, writer, options);
                    writer.Write(",");
                }, o => Serialize(o, writer, options));

                writer.Write("]");
                return;
            }

            //single object
            writer.Write("{");

            var properties = source.GetType()
                .GetProperties();
            
            var entries = new List<PropertyTuple>();
            foreach (var prop in properties)
            {
                PropertyTuple tuple = null;
                var attrib = (JsonPropertyAttribute)prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                    .FirstOrDefault();
                if (attrib is not null && !attrib.Ignore && !options.IgnoreAttributes)
                    tuple = PropertyTuple.Create(options, source, prop.Name, attrib.Name);
                else if (attrib is null || options.IgnoreAttributes)
                    tuple = PropertyTuple.Create(options, source, prop.Name);

                if (tuple is not null && !(options.IgnoreNulls && tuple.Value is null))
                    entries.Add(tuple);
            }

            entries.ProcessList(tuple =>
            {
                writer.Write($"\"{tuple.OutputName}\" : ");
                Serialize(tuple.Value, writer, options);
                writer.Write(",");
            }, tuple =>
            {
                writer.Write($"\"{tuple.OutputName}\" : ");
                Serialize(tuple.Value, writer, options);
            });

            writer.Write("}");
        }
    }
}