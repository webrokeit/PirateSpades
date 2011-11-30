﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    using System.Diagnostics.Contracts;
    using PirateSpades.GameLogic;

    public class PirateHostCommands {
        private static Table pTable = null;

        private static Table Table {
            get {
                return pTable ?? (pTable = Table.GetTable());
            }
        }

        public static void GetPlayerInfo(PirateHost host, PirateClient pclient) {
            Contract.Requires(host != null && pclient != null);
            var msg = new PirateMessage(PirateMessageHead.Pnfo, "");
            host.SendMessage(pclient, msg);
        }

        public static void SetPlayerInfo(PirateHost host, PirateClient pclient, PirateMessage data) {
            Contract.Requires(host != null && data != null && data.Head == PirateMessageHead.Pnfo);
            var player = PirateClient.NameFromString(data.Body);
            if (player == null) return;

            host.SetPlayerName(pclient, player);
        }

        public static void DealCard(PirateHost host, PirateMessage data) {
            Contract.Requires(host != null && data != null && data.Head == PirateMessageHead.Xcrd);
            var player = PirateClient.NameFromString(data.Body);
            if(player == null) return;

            var pclient = host.PlayerFromString(player);
            if(pclient == null) return;

            var card = Card.FromString(data.Body);
            if(card == null) return;

            pclient.ReceiveCard(card);

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

            Table.ReceiveCard(player, card);

            var msg = new PirateMessage(PirateMessageHead.Pcrd, player.ToString() + card.ToString());
            foreach(var pclient in host.GetPlayers()) {
                host.SendMessage(pclient, msg);
            }
        }

        public static void ReceiveBet(PirateHost host, PirateClient player, PirateMessage msg) {
            Contract.Requires(host != null && player != null && msg != null);

            var bet = 0;
            if(int.TryParse(msg.Body, out bet)) {
                player.Bet = bet;
            }

            var betsMade = host.GetPlayers().Count(p => p.Bet > -1);
            if(betsMade >= host.GetPlayers().Count()) {
                // Begin round
            }
        }

        public static void BeginRound(PirateHost host) {
            //Table

        }
    }
}
