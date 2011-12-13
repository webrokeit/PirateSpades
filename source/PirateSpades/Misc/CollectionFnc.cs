// <copyright file="CollectionFnc.cs">
//      ahal@itu.dk,
//      hlck@itu.dk
// </copyright>
// <summary>
//      Contains useful functions for shuffling collections, picking random numbers.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>
// <author>Helena Charlotte Lyn Krüger (hlck@itu.dk)</author>

namespace PirateSpades.Misc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Contains useful functions for shuffling collections, picking random numbers
    /// </summary>
    public static class CollectionFnc {
        /// <summary>
        /// The randomizer to get the pseudo random numbers from.
        /// </summary>
        private static readonly Random R = new Random();
        
        /// <summary>
        /// Shuffles a list of T, using the Fisher Yates algorithm.
        /// http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        /// <typeparam name="T">Anything.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void FisherYatesShuffle<T>(List<T> list) {
            Contract.Requires(list != null);
            Contract.Ensures(list.Count == Contract.OldValue(list.Count));

            if (list.Count < 2) return;

            for (var i = list.Count - 1; i > 0; i--) {
                var index = R.Next(i);

                var tmp = list[index];
                list[index] = list[i];
                list[i] = tmp;
            }
        }

        /// <summary>
        /// Pick a random object.
        /// </summary>
        /// <typeparam name="T">Anything.</typeparam>
        /// <param name="collection">Collection to pick from.</param>
        /// <returns>A random object contained within the colleciton.</returns>
        public static T PickRandom<T>(ICollection<T> collection) {
            Contract.Requires(collection != null && collection.Count > 0);
            return PickRandom(collection.ToList());
        }

        /// <summary>
        /// Pick a random object.
        /// </summary>
        /// <typeparam name="T">Anything.</typeparam>
        /// <param name="collection">Collection to pick from.</param>
        /// <returns>A random object contained within the colleciton.</returns>
        public static T PickRandom<T>(IEnumerable<T> collection) {
            Contract.Requires(collection != null && collection.ToList().Count > 0);
            return PickRandom(collection.ToList());
        }

        /// <summary>
        /// Pick a random object.
        /// </summary>
        /// <typeparam name="T">Anything.</typeparam>
        /// <param name="collection">Collection to pick from.</param>
        /// <returns>A random object contained within the colleciton.</returns>
        public static T PickRandom<T>(List<T> collection) {
            Contract.Requires(collection != null && collection.Count > 0);
            return collection[R.Next(collection.Count)];
        }

        /// <summary>
        /// Pick a random integer between min and max, both included.
        /// </summary>
        /// <param name="min">Minimum number.</param>
        /// <param name="max">Maximum number.</param>
        /// <returns></returns>
        public static int PickRandom(int min, int max) {
            return max < min ? PickRandom(max, min) : R.Next(min, max + 1);
        }
    }
}