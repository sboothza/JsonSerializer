using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Stephen.JsonSerializer
{
    public static class CollectionExtensions
    {
        public static void Delimit(this IEnumerable me,
                                   Action<object, TextWriter> action,
                                   TextWriter writer,
                                   string delimiter)
        {
            var iter = me.GetEnumerator();
            {
                if (iter.MoveNext())
                    action(iter.Current, writer);

                while (iter.MoveNext())
                {
                    writer.Write(delimiter);
                    action(iter.Current, writer);
                }
            }
        }

        public static IEnumerable<T> Cast<T>(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return (T)enumerator.Current;
            }
        }

        public static void Delimit<T>(this IEnumerable<T> me,
                                      Action<T, TextWriter> action,
                                      TextWriter writer,
                                      string delimiter)
        {
            using (var iter = me.GetEnumerator())
            {
                if (iter.MoveNext())
                    action(iter.Current, writer);

                while (iter.MoveNext())
                {
                    writer.Write(delimiter);
                    action(iter.Current, writer);
                }
            }
        }
    }
}