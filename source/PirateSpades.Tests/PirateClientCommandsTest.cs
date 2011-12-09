// <copyright file="PirateClientCommandsTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;
using PirateSpades.GameLogicV2;
using System.Net.Sockets;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateClientCommands))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateClientCommandsTest
    {
        [PexMethod]
        public void VerifyConnection(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.VerifyConnection(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.VerifyConnection(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void SetBet(PirateClient pclient, int bet) {
            PirateClientCommands.SetBet(pclient, bet);
            // TODO: add assertions to method PirateClientCommandsTest.SetBet(PirateClient, Int32)
        }
        [PexMethod]
        public void SendPlayerInfo(PirateClient pclient) {
            PirateClientCommands.SendPlayerInfo(pclient);
            // TODO: add assertions to method PirateClientCommandsTest.SendPlayerInfo(PirateClient)
        }
        [PexMethod]
        public void PlayCard(PirateClient pclient, Card card) {
            PirateClientCommands.PlayCard(pclient, card);
            // TODO: add assertions to method PirateClientCommandsTest.PlayCard(PirateClient, Card)
        }
        [PexMethod]
        public void NewRound(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.NewRound(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.NewRound(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void NewPile(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.NewPile(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.NewPile(PirateClient, PirateMessage)
        }
        [PexMethod]
        public bool KnockKnock(Socket client) {
            bool result = PirateClientCommands.KnockKnock(client);
            return result;
            // TODO: add assertions to method PirateClientCommandsTest.KnockKnock(Socket)
        }
        [PexMethod]
        public void InitConnection(PirateClient pclient) {
            PirateClientCommands.InitConnection(pclient);
            // TODO: add assertions to method PirateClientCommandsTest.InitConnection(PirateClient)
        }
        [PexMethod]
        public void GetPlayersInGame(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.GetPlayersInGame(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.GetPlayersInGame(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void GetPlayedCard(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.GetPlayedCard(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.GetPlayedCard(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void GetCard(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.GetCard(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.GetCard(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void GameStarted(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.GameStarted(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.GameStarted(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void GameFinished(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.GameFinished(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.GameFinished(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void FinishRound(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.FinishRound(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.FinishRound(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void ErrorMessage(PirateClient pclient, PirateMessage msg) {
            PirateClientCommands.ErrorMessage(pclient, msg);
            // TODO: add assertions to method PirateClientCommandsTest.ErrorMessage(PirateClient, PirateMessage)
        }
        [PexMethod]
        public void DealCard(
            PirateClient pclient,
            Player receiver,
            Card card
        ) {
            PirateClientCommands.DealCard(pclient, receiver, card);
            // TODO: add assertions to method PirateClientCommandsTest.DealCard(PirateClient, Player, Card)
        }
        [PexMethod]
        public void BeginRound(PirateClient pclient, PirateMessage data) {
            PirateClientCommands.BeginRound(pclient, data);
            // TODO: add assertions to method PirateClientCommandsTest.BeginRound(PirateClient, PirateMessage)
        }
    }
}
