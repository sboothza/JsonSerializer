using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stephen.JsonSerializer
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> Cast<T>(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return (T)enumerator.Current;
            }
        }

        // public static void DelimitToWriter<T>(this IEnumerable<T> me, Action<T, LayoutStreamWriter> action, LayoutStreamWriter writer,
        //     string delimiter)
        // {
        //     using (var iter = me.GetEnumerator())
        //     {
        //         if (iter.MoveNext())
        //             action(iter.Current, writer);
        //
        //         while (iter.MoveNext())
        //         {
        //             writer.Write(delimiter);
        //             action(iter.Current, writer);
        //         }
        //     }
        // }

        public static void ProcessList<T>(this IEnumerable<T> enumerable, Action<T> action, Action<T> lastAction)
        {
            try
            {
                using (var enu = enumerable.GetEnumerator())
                {
                    bool canMove = enu.MoveNext();
                    T last =default;
                    while (canMove)
                    {
                        last = enu.Current;
                        canMove = enu.MoveNext();
                        if (canMove)
                            action(last);
                    } 
                    
                    if (last != null)
                        lastAction(last);
                }
                // if (!enumerable.Any())
                //     return;
                // foreach (var element in enumerable.SkipLast(1))
                //     action(element);
                // var last = enumerable.LastOrDefault();
                // if (last != null)
                //     lastAction(last);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}