using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PokeGoBot.Core.Logging.Helpers
{
    public static class Sorting
    {
        public static void SortByDesc<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            var sortedList = source.OrderByDescending(keySelector).ToList();
            source.Clear();
            foreach (var item in sortedList)
                source.Add(item);
        }
    }
}
