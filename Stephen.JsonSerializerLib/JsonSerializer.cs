using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Stephen.JsonSerializer
{
    public static class JsonSerializer
    {
        public static string Serialize(object source, JsonSerializerOptions options = null)
        {
            var writer = new StringWriter(new StringBuilder(1024));
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
                    writer.Write("{");

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
                writer.Write("[");

                sourceEnumerable.Cast<object>()
                                .Where(i => !options.IgnoreTypes.Any(t => t.IsInstanceOfType(i)))
                                .DelimitToWriter((entry, wr) => Serialize(entry, wr, options), writer, ",");

                writer.Write("]");
                return;
            }

            //single object
            writer.Write("{");

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

            writer.Write("}");
        }
    }
}