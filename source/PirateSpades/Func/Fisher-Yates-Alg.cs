// Helena 

namespace PirateSpades.Func
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A shuffle algorithm, which is unbiased. Iterates through the whole collection and swaps the item at index i, with an item 
    /// at a randomly generated index in the collection. Does this until there's only one item left.
    /// </summary>
    public class FisherYatesAlg
    {
        private Random r;

        public FisherYatesAlg() {
            this.r = new Random();
        }

        public void Algorithm<T>(IList<T> c) {
            Contract.Requires(c.Count != 0);
            Contract.Ensures(c.Count == Contract.OldValue(c.Count));

            if (c.Count > 1) {
                int i = c.Count - 1;
                while (i > 1) {
                    int s = r.Next(i);
                    T holder = c[i];
                    c[i] = c[s];
                    c[s] = holder;
                    i--;
                }
            }
        }
    }
}
