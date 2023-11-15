using System;
using System.Collections;
using System.Collections.Generic;

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}