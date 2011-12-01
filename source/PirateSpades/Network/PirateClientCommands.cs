// <copyright file="PirateClientCommands.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Various commands used by the PirateClientt to communicate with its host (PirateHost).
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpades.Network {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics.Contracts;
    using PirateSpades.GameLogic;

    public class PirateClientCommands {
        private static Table pTable = null;

        private static Table Table {
            get {
                return pTable ?? (pTable = Table.GetTable());
            }
        }

        public static void InitConnection(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Init, "");
            pclient.SendMessage(msg);
        }

        public static void VerifyConnection(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null && data.Head == PirateMessageHead.Init);
            var msg = new PirateMessage(PirateMessageHead.Verf, data.Body);
            pclient.SendMessage(msg);
        }

        public static void SendPlayerInfo(PirateClient pclient) {
            Contract.Requires(pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Pnfo, pclient.ToString());
            pclient.SendMessage(msg);
        }

        public static void GetPlayersInGame(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null);

            Table.ClearPlayers();
            var players = PirateClient.NamesFromString(data.Body);
            if(players.Count > 0) {
                Console.WriteLine("Current players in game:");
               foreach(var player in players) {
                   Table.AddPlayer(new Player(player));
                   Console.WriteLine("\t" + player + (pclient.Name == player ? " (YOU)" : ""));
               } 
            }
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

            Console.WriteLine(pclient.Name + ": Dealing " + card + " to " + receiver.Name);

            //Table.GetPlayers().First(player => player.Name == receiver.Name).ReceiveCard(card);

            pclient.SendMessage(msg);
        }

        public static void GetCard(PirateClient pclient, PirateMessage data) {
            Contract.Requires(pclient != null && data != null);
            var card = Card.FromString(data.Body);
            if(card == null) return;

            Console.WriteLine(pclient.Name + ": Received " + card);

            //pclient.ReceiveCard(card);
        }

        public static void SetBet(PirateClient pclient, int bet) {
            Contract.Requires(pclient != null & bet >= 0);

            var msg = new PirateMessage(PirateMessageHead.Satk, bet.ToString());
            pclient.SendMessage(msg);
        }

        public static void GetPlayerBets(PirateMessage msg) {
            Contract.Requires(msg != null);
            var bets = PirateMessage.GetPlayerBets(msg);

        }
    }
}
