using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stephen.JsonSerializer
{
    public class JsonSerializer
    {
        public List<Type> IgnoreTypes { get; }

        public JsonSerializer(params Type[] ignoreTypes) { IgnoreTypes = new List<Type>(ignoreTypes); }

        public string Serialize(object source)
        {
            var writer = new StringWriter(new StringBuilder(1024));
            Serialize(source, writer);
            return writer.ToString();
        }

        public void Serialize(object source, TextWriter writer)
        {
            if (source is null)
            {
                writer.Write("null");
                return;
            }

            if (IgnoreTypes.Any(t => t.IsInstanceOfType(source)))
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
                                    .Where(p => !IgnoreTypes.Any(t => t.IsInstanceOfType(p.Value)))
                                    .Delimit((entry, wr) =>
                                             {
                                                 wr.Write($"\"{entry.Key}\":");
                                                 Serialize(entry.Value, wr);
                                             },
                                             writer,
                                             ",");

                    writer.Write("}");
                    return;
                }

                //handle list
                writer.Write("[");

                sourceEnumerable.Cast<object>()
                                .Where(i => !IgnoreTypes.Any(t => t.IsInstanceOfType(i)))
                                .Delimit(Serialize,
                                         writer,
                                         ",");

                writer.Write("]");
                return;
            }

            //single object
            writer.Write("{");

            source.GetType()
                  .GetProperties()
                  .Select(prop => new
                                  {
                                      Name = prop.Name,
                                      Value = source.GetFieldOrPropertyValue(prop.Name)
                                  })
                  .Where(p => !IgnoreTypes.Any(t => t.IsInstanceOfType(p.Value)))
                  .Delimit((item, wr) =>
                           {
                               wr.Write($"\"{item.Name}\":");
                               Serialize(item.Value, wr);
                           },
                           writer,
                           ",");

            writer.Write("}");
        }
    }
}