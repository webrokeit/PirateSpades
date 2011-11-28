using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.GameLogic {
    public class Table {
        //MAKE SINGLETON
        private static Card open;

        public static Card OpeningCard { get { return open; } }

        public static void ReceiveCard(Player p) {
            
        }
    }
}
