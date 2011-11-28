using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.GameLogic {
    public class Table {
        //MAKE SINGLETON
        private Card open;
        private static readonly Table Me = new Table();

        private Table() {
            
        }

        public static Table GetTable() {
            return Me;
        }

        public Card OpeningCard { get { return open; } }

        public static void ReceiveCard(Player p) {
            
        }
    }
}
