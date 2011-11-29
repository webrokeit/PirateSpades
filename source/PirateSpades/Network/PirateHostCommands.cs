using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    using System.Diagnostics.Contracts;

    using PirateSpades.GameLogic;

    public class PirateHostCommands {
        public static void DealCard(PirateHost host, PirateMessage data) {
            Contract.Requires(host != null && data != null && data.Head == PirateMessageHead.Xcrd);
            var player = PirateClient.NameFromString(data.Body);
            if(player == null) return;

            var pclient = host.PlayerFromString(player);
            if(pclient == null) return;

            var card = Card.FromString(data.Body);
            if(card == null) return;

            var msg = new PirateMessage(PirateMessageHead.Xcrd, card.ToString());
            host.SendMessage(pclient, msg);
        }

        public static void PlayCard(PirateHost host, PirateMessage data) {
            Contract.Requires(host != null && data != null && data.Head == PirateMessageHead.Xcrd);
            var playerName = PirateClient.NameFromString(data.Body);
            if(playerName == null)
                return;

            var player = host.PlayerFromString(playerName);
            if(player == null)
                return;

            var card = Card.FromString(data.Body);
            if(card == null)
                return;

            var msg = new PirateMessage(PirateMessageHead.Pcrd, player.ToString() + card.ToString());
            foreach(var pclient in host.GetPlayers()) {
                host.SendMessage(pclient, msg);
            }
        }

        
    }
}
