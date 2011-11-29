// Helena 

namespace PirateSpades.Func
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using PirateSpades.GameLogic;

    /// <summary>
    /// Sorts the hand. Parts it into suits and afterwards value. 
    /// </summary>
    public class Sorting
    {
        /// <summary>
        /// We make a dictionary, with the suits as keys and a list of cards as value. First we sort them by the keys, then by the value and at last 
        /// we gather all the items in the dictionary and put them in our hand. 
        /// </summary>
        public static void SortHand(List<Card> l) {
            Contract.Requires(l != null & l.Count > 0);
            var c = new Dictionary<Suit, List<Card>>
                {
                    {Suit.Diamonds, new List<Card>() },
                    {Suit.Clubs, new List<Card>() },
                    {Suit.Hearts, new List<Card>() },
                    {Suit.Spades, new List<Card>() },
                };

            
            foreach (var card in l) {
                c[card.Suit].Add(card);
            }

            foreach (var lc in c.Values) {
                lc.Sort();
            }

            l = new List<Card>(c[Suit.Diamonds].Concat(c[Suit.Clubs]).Concat(c[Suit.Hearts]).Concat(c[Suit.Spades]));
        }
    }
}
