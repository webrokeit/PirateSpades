﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PirateSpades.GameLogic {
    using System;

    public class Table {
        private Card open;
        private static readonly Table Me = new Table();
        private List<Player> players;
        private Player winning;
        private List<Card> cards;
        private Card winningCard;
        private Dictionary<Player, Card> playedCards;
        private int CurrentPlayerIndex = 0;
        private bool started;

        private Table() {
            open = null;
            winning = null;
            cards = new List<Card>();
            CardsPlayed = 0;
            playedCards = new Dictionary<Player, Card>();
            players = new List<Player>();
        }

        public static Table GetTable() {
            return Me;
        }

        public Game Game { get; private set; }

        public int Players { get { return players.Count; } }

        public int Cards { get { return cards.Count; } }

        public Card OpeningCard { get { return open; } }

        public Player Winning { get { return winning; } }

        public int CardsPlayed { get; set; }

        public Player PlayerTurn { get; set; }

        public Player StartingPlayer { get; set; }

        public bool IsStarted { get { return started; } }

        public bool SameSuit(Player p, Card c) {
            Contract.Requires(c != null && p != null);
            Contract.Ensures(c.Suit == OpeningCard.Suit || !p.AnyCard(OpeningCard.Suit) ? Contract.Result<bool>() : true);
            return p.Playable(c);
        }

        public void AddPlayers(List<Player> p) {
            Contract.Requires(p != null);
            Contract.Ensures(Players == p.Count);
            players = new List<Player>(p);
        }

        public IEnumerable<Player> GetPlayers() {
            Contract.Requires(Players > 1);
            return this.players;
        }

        public void ReceiveCard(Player p, Card c) {
            Contract.Requires(StartingPlayer == PlayerTurn || this.SameSuit(p, c));
            Contract.Requires(p != null && c != null && !this.IsRoundFinished() && PlayerTurn == p && IsStarted);
            if(open == null) {
                CurrentPlayerIndex = (players.IndexOf(p) + 1) % players.Count;
                playedCards.Clear();
                cards.Add(c);
                CardsPlayed++;
                open = c;
                playedCards.Add(p, c);
                PlayerTurn = players[CurrentPlayerIndex];
                return;
            }
            cards.Add(c);
            CardsPlayed++;
            playedCards.Add(p, c);
            if(!this.IsRoundFinished()) {
                CurrentPlayerIndex = (CurrentPlayerIndex +1) % players.Count;
                PlayerTurn = players[CurrentPlayerIndex];
                return;
            }
            this.FinishRound();
        }

        public void Start(Game g) {
            Contract.Requires(this.GetPlayers().All(p => p.Bet >= 0));
            this.Game = g;
            started = true;
        }

        public void Stop() {
            Contract.Ensures(!IsStarted);
            started = false;
            CardsPlayed = 0;
        }

        public bool IsRoundFinished() {
            Contract.Ensures(Cards == Players ? Contract.Result<bool>() : true);
            return Cards == Players;
        }

        public void FinishRound() {
            Contract.Requires(this.IsRoundFinished());
            Contract.Ensures(OpeningCard == null && Cards == 0);
            foreach (var kvp in this.playedCards.Where(kvp => this.winning == null || this.winningCard.CompareTo(kvp.Value) < 0)) {
                this.winning = kvp.Key;
                this.winningCard = kvp.Value;
            }
            winning.ReceiveTrick(cards);
            cards.Clear();
            PlayerTurn = Winning;
            StartingPlayer = Winning;
            winning = null;
            winningCard = null;
            open = null;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(Players <= 5);
            Contract.Invariant(Cards >= 0 && Cards <= Players);
        }
    }
}
