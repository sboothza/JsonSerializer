using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stephen.JsonSerializer
{
    public static class JsonSerializer
    {
        public static string Serialize(object source, JsonSerializerOptions options = null)
        {
            var writer = new LayoutStreamWriter();
            Serialize(source, writer, options, false);
            return writer.ToString();
        }

        private static void Serialize(object source, LayoutStreamWriter writer, JsonSerializerOptions options,
            bool sameLine)
        {
            if (source is null)
            {
                writer.Write("null", sameLine);
                return;
            }

            options ??= new JsonSerializerOptions();

            if (options.IgnoreTypes.Any(t => t.IsInstanceOfType(source)))
                return;

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

                    if (options.Pretty)
                        writer.WriteLine("{");
                    else
                        writer.Write("{");

                    using (writer.StartBlock(options.Pretty))
                    {
                        var entries = dictionarySource.GetEnumerator()
                            .Cast<DictionaryEntry>()
                            .Where(p => !options.IgnoreTypes.Any(t => t.IsInstanceOfType(p.Value)));
                        entries.ProcessList(entry =>
                        {
                            writer.Write($"\"{entry.Key}\" : ");
                            Serialize(entry.Value, writer, options, true);
                            if (options.Pretty)
                                writer.WriteLine(",");
                            else
                                writer.Write(",");
                        }, entry =>
                        {
                            writer.Write($"\"{entry.Key}\" : ");
                            Serialize(entry.Value, writer, options, true);
                        });
                    }

                    if (options.Pretty)
                        writer.WriteLine();

                    writer.Write("}");
                    return;
                }

                //handle list
                if (options.Pretty)
                    writer.WriteLine("[");
                else
                    writer.Write("[");

                using (writer.StartBlock(options.Pretty))
                {
                    var entries = sourceEnumerable.Cast<object>()
                        .Where(i => !options.IgnoreTypes.Any(t => t.IsInstanceOfType(i)));

                    entries.ProcessList(o =>
                    {
                        Serialize(o, writer, options, false);
                        if (options.Pretty)
                            writer.WriteLine(",", false);
                        else
                            writer.Write(",");
                    }, o => Serialize(o, writer, options, false));
                }

                if (options.Pretty)
                    writer.WriteLine();
                writer.Write("]");
                return;
            }

            //single object
            if (options.Pretty)
                writer.WriteLine("{");
            else
                writer.Write("{");

            using (writer.StartBlock(options.Pretty))
            {
                var properties = source.GetType()
                    .GetProperties()
                    .Where(p => options.IgnoreTypes.All(t => t != p.PropertyType));

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

                // var entries = properties
                //                     .Select(prop => PropertyTuple.Create(options, source, prop.Name))
                //                     .Where(p => !options.IgnoreTypes.Any(t => t.IsInstanceOfType(p.Value)))
                //                     .Where(p => p != null)
                //                     .Where(p => !options.IgnoreNulls || p.Value is not null);

                entries.ProcessList(tuple =>
                {
                    writer.Write($"\"{tuple.OutputName}\" : ");
                    Serialize(tuple.Value, writer, options, true);
                    if (options.Pretty)
                        writer.WriteLine(",", false);
                    else
                        writer.Write(",");
                }, tuple =>
                {
                    writer.Write($"\"{tuple.OutputName}\" : ");
                    Serialize(tuple.Value, writer, options, true);
                });
            }

            if (options.Pretty)
                writer.WriteLine();
            writer.Write("}");
        }
    }
}