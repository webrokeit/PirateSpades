using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    using System.Diagnostics.Contracts;
    using PirateSpades.GameLogic;

    public class PirateClientCommands {
        public static void SendPlayerInfo(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Pnfo, pclient.ToString());
            pclient.SendMessage(msg);
        }

        public static void PlayCard(PirateClient pclient, Card card) {
            Contract.Requires(pclient != null && card != null);
            var body = PirateMessage.ConstructBody(pclient.ToString(), card.ToString());
            var msg = new PirateMessage(PirateMessageHead.Pcrd, body);
            pclient.SendMessage(msg);
        }

        public static void DealCard(PirateClient pclient, Player receiver, Card card) {
            Contract.Requires(pclient != null && receiver != null && card != null);
            var body = PirateMessage.ConstructBody(PirateClient.NameToString(receiver.Name), card.ToString());
            var msg = new PirateMessage(PirateMessageHead.Xcrd, body);
            pclient.SendMessage(msg);
        }

        public static void GetCard(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null);

            var card = Card.FromString(data.Body);
            if(card == null) return;

            pclient.ReceiveCard(card);
        }

        public static void SetBet(PirateClient pclient, int bet) {
            Contract.Requires(pclient != null & bet >= 0);

            var msg = new PirateMessage(PirateMessageHead.Satk, bet.ToString());
            pclient.SendMessage(msg);
        }
    }
}
