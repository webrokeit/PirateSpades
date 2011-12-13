// Helena 
namespace PirateSpades.Misc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// A shuffle algorithm, which is unbiased. Iterates through the whole collection and swaps the item at index i, with an item 
    /// at a randomly generated index in the collection. Does this until there's only one item left.
    /// </summary>
    public class CollectionFnc
    {
        private static readonly Random R = new Random();

        public static void FisherYatesShuffle<T>(List<T> list) {
            Contract.Requires(list != null && list.Count > 0);
            Contract.Ensures(list.Count == Contract.OldValue(list.Count));

            if(list.Count < 2) return;

            for(var i = list.Count - 1; i > 0; i--) {
                var index = R.Next(i);

                var tmp = list[index];
                list[index] = list[i];
                list[i] = tmp;
            }
        }

        public static T PickRandom<T>(ICollection<T> collection) {
            Contract.Requires(collection != null && collection.Count > 0);
            return PickRandom(collection.ToList());
        }

        public static T PickRandom<T>(IEnumerable<T> collection) {
            Contract.Requires(collection != null && collection.ToList().Count > 0);
            return PickRandom(collection.ToList());
        }

        public static T PickRandom<T>(List<T> collection) {
            Contract.Requires(collection != null && collection.Count > 0);
            return collection[R.Next(collection.Count)];
        }

        public static int PickRandom(int min, int max) {
            return max < min ? PickRandom(max, min) : R.Next(min, max + 1);
        }
    }
}
