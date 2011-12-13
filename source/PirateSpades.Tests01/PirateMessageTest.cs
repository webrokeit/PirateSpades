// <copyright file="PirateMessageTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;
using System.Collections.Generic;
using System.Net;
using PirateSpades.GameLogic;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateMessage))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateMessageTest
    {
        [PexMethod]
        public string GetWinner(PirateMessage msg) {
            string result = PirateMessage.GetWinner(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetWinner(PirateMessage)
        }
        [PexMethod]
        public string GetStartingPlayer(PirateMessage msg) {
            string result = PirateMessage.GetStartingPlayer(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetStartingPlayer(PirateMessage)
        }
        [PexMethod]
        public int GetRound(PirateMessage msg) {
            int result = PirateMessage.GetRound(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetRound(PirateMessage)
        }
        [PexMethod]
        public int GetPlayersInGame(PirateMessage msg) {
            int result = PirateMessage.GetPlayersInGame(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetPlayersInGame(PirateMessage)
        }
        [PexMethod]
        public Dictionary<string, int> GetPlayerTricks(PirateMessage msg) {
            Dictionary<string, int> result = PirateMessage.GetPlayerTricks(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetPlayerTricks(PirateMessage)
        }
        [PexMethod]
        public Dictionary<string, int> GetPlayerScores(PirateMessage msg) {
            Dictionary<string, int> result = PirateMessage.GetPlayerScores(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetPlayerScores(PirateMessage)
        }
        [PexMethod]
        public HashSet<string> GetPlayerNames(PirateMessage msg) {
            HashSet<string> result = PirateMessage.GetPlayerNames(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetPlayerNames(PirateMessage)
        }
        [PexMethod]
        public string GetPlayerName(PirateMessage msg) {
            string result = PirateMessage.GetPlayerName(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetPlayerName(PirateMessage)
        }
        [PexMethod]
        public Dictionary<string, int> GetPlayerBets(PirateMessage msg) {
            Dictionary<string, int> result = PirateMessage.GetPlayerBets(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetPlayerBets(PirateMessage)
        }
        [PexMethod]
        public List<PirateMessage> GetMessages(byte[] buffer, int readLen) {
            List<PirateMessage> result = PirateMessage.GetMessages(buffer, readLen);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetMessages(Byte[], Int32)
        }
        [PexMethod]
        public int GetMaxPlayersInGame(PirateMessage msg) {
            int result = PirateMessage.GetMaxPlayersInGame(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetMaxPlayersInGame(PirateMessage)
        }
        [PexMethod]
        public IPAddress GetHostIp(PirateMessage msg) {
            IPAddress result = PirateMessage.GetHostIp(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetHostIp(PirateMessage)
        }
        [PexMethod]
        public string GetGameName(PirateMessage msg) {
            string result = PirateMessage.GetGameName(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetGameName(PirateMessage)
        }
        [PexMethod]
        public PirateError GetError(string s) {
            PirateError result = PirateMessage.GetError(s);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetError(String)
        }
        [PexMethod]
        public string GetDealer(PirateMessage msg) {
            string result = PirateMessage.GetDealer(msg);
            return result;
            // TODO: add assertions to method PirateMessageTest.GetDealer(PirateMessage)
        }
        [PexMethod]
        public byte[] GetBytes([PexAssumeUnderTest]PirateMessage target) {
            byte[] result = target.GetBytes();
            return result;
            // TODO: add assertions to method PirateMessageTest.GetBytes(PirateMessage)
        }
        [PexMethod]
        public IList<string> ContstructPlayerScores(Dictionary<Player, int> scores) {
            IList<string> result = PirateMessage.ContstructPlayerScores(scores);
            return result;
            // TODO: add assertions to method PirateMessageTest.ContstructPlayerScores(Dictionary`2<Player,Int32>)
        }
        [PexMethod]
        public PirateMessage Constructor01(PirateMessageHead head, string body) {
            PirateMessage target = new PirateMessage(head, body);
            return target;
            // TODO: add assertions to method PirateMessageTest.Constructor01(PirateMessageHead, String)
        }
        [PexMethod]
        public PirateMessage Constructor(string head, string body) {
            PirateMessage target = new PirateMessage(head, body);
            return target;
            // TODO: add assertions to method PirateMessageTest.Constructor(String, String)
        }
        [PexMethod]
        public string ConstructWinner(Player player) {
            string result = PirateMessage.ConstructWinner(player);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructWinner(Player)
        }
        [PexMethod]
        public string ConstructStartingPlayer(Player player) {
            string result = PirateMessage.ConstructStartingPlayer(player);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructStartingPlayer(Player)
        }
        [PexMethod]
        public string ConstructRoundNumber(int round) {
            string result = PirateMessage.ConstructRoundNumber(round);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructRoundNumber(Int32)
        }
        [PexMethod]
        public string ConstructPlayersInGame(int players) {
            string result = PirateMessage.ConstructPlayersInGame(players);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructPlayersInGame(Int32)
        }
        [PexMethod]
        public IList<string> ConstructPlayerTricks(Round round) {
            IList<string> result = PirateMessage.ConstructPlayerTricks(round);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructPlayerTricks(Round)
        }
        [PexMethod]
        public string ConstructPlayerTrick(Round round, Player player) {
            string result = PirateMessage.ConstructPlayerTrick(round, player);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructPlayerTrick(Round, Player)
        }
        [PexMethod]
        public string ConstructPlayerScore(Player player, int score) {
            string result = PirateMessage.ConstructPlayerScore(player, score);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructPlayerScore(Player, Int32)
        }
        [PexMethod]
        public string ConstructPlayerName01(string name) {
            string result = PirateMessage.ConstructPlayerName(name);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructPlayerName01(String)
        }
        [PexMethod]
        public string ConstructPlayerName(Player player) {
            string result = PirateMessage.ConstructPlayerName(player);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructPlayerName(Player)
        }
        [PexMethod]
        public string ConstructPlayerBet(Player player) {
            string result = PirateMessage.ConstructPlayerBet(player);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructPlayerBet(Player)
        }
        [PexMethod]
        public string ConstructMaxPlayersInGame(int players) {
            string result = PirateMessage.ConstructMaxPlayersInGame(players);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructMaxPlayersInGame(Int32)
        }
        [PexMethod]
        public string ConstructHostIp(PirateHost host) {
            string result = PirateMessage.ConstructHostIp(host);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructHostIp(PirateHost)
        }
        [PexMethod]
        public string ConstructHostInfo(PirateHost host) {
            string result = PirateMessage.ConstructHostInfo(host);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructHostInfo(PirateHost)
        }
        [PexMethod]
        public string ConstructGameName(PirateHost host) {
            string result = PirateMessage.ConstructGameName(host);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructGameName(PirateHost)
        }
        [PexMethod]
        public string ConstructDealer(string name) {
            string result = PirateMessage.ConstructDealer(name);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructDealer(String)
        }
        [PexMethod]
        public string ConstructBody01(IEnumerable<string> inputs) {
            string result = PirateMessage.ConstructBody(inputs);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructBody01(IEnumerable`1<String>)
        }
        [PexMethod]
        public string ConstructBody(string[] inputs) {
            string result = PirateMessage.ConstructBody(inputs);
            return result;
            // TODO: add assertions to method PirateMessageTest.ConstructBody(String[])
        }
        [PexMethod]
        public string AppendBody(string body, string[] inputs) {
            string result = PirateMessage.AppendBody(body, inputs);
            return result;
            // TODO: add assertions to method PirateMessageTest.AppendBody(String, String[])
        }
    }
}
