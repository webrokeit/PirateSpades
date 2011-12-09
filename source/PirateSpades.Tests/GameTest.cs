// <copyright file="GameTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.GameLogicV2;
using System.Collections.Generic;

namespace PirateSpades.GameLogicV2
{
    using PirateSpades.GameLogic;

    [TestClass]
    [PexClass(typeof(Game))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class GameTest
    {
        [PexMethod]
        public void Start01(
            [PexAssumeUnderTest]Game target,
            bool isHost,
            int dealer
        ) {
            target.Start(isHost, dealer);
            // TODO: add assertions to method GameTest.Start01(Game, Boolean, Int32)
        }
        [PexMethod]
        public void Start([PexAssumeUnderTest]Game target, int dealer) {
            target.Start(dealer);
            // TODO: add assertions to method GameTest.Start(Game, Int32)
        }
        [PexMethod]
        public int RoundsPossibleGet([PexAssumeUnderTest]Game target) {
            int result = target.RoundsPossible;
            return result;
            // TODO: add assertions to method GameTest.RoundsPossibleGet(Game)
        }
        [PexMethod]
        public Round RoundGet([PexAssumeUnderTest]Game target) {
            Round result = target.Round;
            return result;
            // TODO: add assertions to method GameTest.RoundGet(Game)
        }
        [PexMethod]
        public void RemovePlayer([PexAssumeUnderTest]Game target, Player player) {
            target.RemovePlayer(player);
            // TODO: add assertions to method GameTest.RemovePlayer(Game, Player)
        }
        [PexMethod]
        public int PlayerIndex01([PexAssumeUnderTest]Game target, string playerName) {
            int result = target.PlayerIndex(playerName);
            return result;
            // TODO: add assertions to method GameTest.PlayerIndex01(Game, String)
        }
        [PexMethod]
        public int PlayerIndex([PexAssumeUnderTest]Game target, Player player) {
            int result = target.PlayerIndex(player);
            return result;
            // TODO: add assertions to method GameTest.PlayerIndex(Game, Player)
        }
        [PexMethod]
        public void NewRound([PexAssumeUnderTest]Game target) {
            target.NewRound();
            // TODO: add assertions to method GameTest.NewRound(Game)
        }
        [PexMethod]
        public Player LeaderGet([PexAssumeUnderTest]Game target) {
            Player result = target.Leader;
            return result;
            // TODO: add assertions to method GameTest.LeaderGet(Game)
        }
        [PexMethod]
        public Dictionary<Player, int> GetTotalScores([PexAssumeUnderTest]Game target) {
            Dictionary<Player, int> result = target.GetTotalScores();
            return result;
            // TODO: add assertions to method GameTest.GetTotalScores(Game)
        }
        [PexMethod]
        public Dictionary<Player, int> GetRoundScoreTotal([PexAssumeUnderTest]Game target, int roundNum) {
            Dictionary<Player, int> result = target.GetRoundScoreTotal(roundNum);
            return result;
            // TODO: add assertions to method GameTest.GetRoundScoreTotal(Game, Int32)
        }
        [PexMethod]
        public Dictionary<Player, int> GetRoundScore([PexAssumeUnderTest]Game target, int round) {
            Dictionary<Player, int> result = target.GetRoundScore(round);
            return result;
            // TODO: add assertions to method GameTest.GetRoundScore(Game, Int32)
        }
        [PexMethod]
        public Round GetRound([PexAssumeUnderTest]Game target, int round) {
            Round result = target.GetRound(round);
            return result;
            // TODO: add assertions to method GameTest.GetRound(Game, Int32)
        }
        [PexMethod]
        public Player GetPlayer([PexAssumeUnderTest]Game target, string playerName) {
            Player result = target.GetPlayer(playerName);
            return result;
            // TODO: add assertions to method GameTest.GetPlayer(Game, String)
        }
        [PexMethod]
        public bool FinishedGet([PexAssumeUnderTest]Game target) {
            bool result = target.Finished;
            return result;
            // TODO: add assertions to method GameTest.FinishedGet(Game)
        }
        [PexMethod]
        public bool Contains01([PexAssumeUnderTest]Game target, string playerName) {
            bool result = target.Contains(playerName);
            return result;
            // TODO: add assertions to method GameTest.Contains01(Game, String)
        }
        [PexMethod]
        public bool Contains([PexAssumeUnderTest]Game target, Player player) {
            bool result = target.Contains(player);
            return result;
            // TODO: add assertions to method GameTest.Contains(Game, Player)
        }
        [PexMethod]
        public Game Constructor01(IEnumerable<Player> players) {
            Game target = new Game(players);
            return target;
            // TODO: add assertions to method GameTest.Constructor01(IEnumerable`1<Player>)
        }
        [PexMethod]
        public Game Constructor() {
            Game target = new Game();
            return target;
            // TODO: add assertions to method GameTest.Constructor()
        }
        [PexMethod]
        public void ClearPlayers([PexAssumeUnderTest]Game target) {
            target.ClearPlayers();
            // TODO: add assertions to method GameTest.ClearPlayers(Game)
        }
        [PexMethod]
        public int CardsToDealGet([PexAssumeUnderTest]Game target) {
            int result = target.CardsToDeal;
            return result;
            // TODO: add assertions to method GameTest.CardsToDealGet(Game)
        }
        [PexMethod]
        public void AddPlayers([PexAssumeUnderTest]Game target, IEnumerable<Player> players) {
            target.AddPlayers(players);
            // TODO: add assertions to method GameTest.AddPlayers(Game, IEnumerable`1<Player>)
        }
        [PexMethod]
        public void AddPlayer([PexAssumeUnderTest]Game target, Player player) {
            target.AddPlayer(player);
            // TODO: add assertions to method GameTest.AddPlayer(Game, Player)
        }
        [PexMethod]
        public bool ActiveGet([PexAssumeUnderTest]Game target) {
            bool result = target.Active;
            return result;
            // TODO: add assertions to method GameTest.ActiveGet(Game)
        }
    }
}
