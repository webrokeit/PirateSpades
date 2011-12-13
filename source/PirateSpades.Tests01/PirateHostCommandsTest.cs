// <copyright file="PirateHostCommandsTest.cs">Copyright ©  2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PirateSpades.Network;

namespace PirateSpades.Network
{
    [TestClass]
    [PexClass(typeof(PirateHostCommands))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class PirateHostCommandsTest
    {
        [PexMethod]
        public void VerifyConnection(
            PirateHost host,
            PirateClient pclient,
            PirateMessage data
        ) {
            PirateHostCommands.VerifyConnection(host, pclient, data);
            // TODO: add assertions to method PirateHostCommandsTest.VerifyConnection(PirateHost, PirateClient, PirateMessage)
        }
        [PexMethod]
        public void StartGame(PirateHost host) {
            PirateHostCommands.StartGame(host);
            // TODO: add assertions to method PirateHostCommandsTest.StartGame(PirateHost)
        }
        [PexMethod]
        public void SetPlayerInfo(
            PirateHost host,
            PirateClient pclient,
            PirateMessage data
        ) {
            PirateHostCommands.SetPlayerInfo(host, pclient, data);
            // TODO: add assertions to method PirateHostCommandsTest.SetPlayerInfo(PirateHost, PirateClient, PirateMessage)
        }
        [PexMethod]
        public void SendPlayerInfo(PirateHost host) {
            PirateHostCommands.SendPlayerInfo(host);
            // TODO: add assertions to method PirateHostCommandsTest.SendPlayerInfo(PirateHost)
        }
        [PexMethod]
        public void RoundFinished(PirateHost host) {
            PirateHostCommands.RoundFinished(host);
            // TODO: add assertions to method PirateHostCommandsTest.RoundFinished(PirateHost)
        }
        [PexMethod]
        public void RequestCard(PirateHost host, PirateClient pclient) {
            PirateHostCommands.RequestCard(host, pclient);
            // TODO: add assertions to method PirateHostCommandsTest.RequestCard(PirateHost, PirateClient)
        }
        [PexMethod]
        public void RequestBets(PirateHost host) {
            PirateHostCommands.RequestBets(host);
            // TODO: add assertions to method PirateHostCommandsTest.RequestBets(PirateHost)
        }
        [PexMethod]
        public void ReceiveBet(
            PirateHost host,
            PirateClient player,
            PirateMessage msg
        ) {
            PirateHostCommands.ReceiveBet(host, player, msg);
            // TODO: add assertions to method PirateHostCommandsTest.ReceiveBet(PirateHost, PirateClient, PirateMessage)
        }
        [PexMethod]
        public void PlayCard(PirateHost host, PirateMessage data) {
            PirateHostCommands.PlayCard(host, data);
            // TODO: add assertions to method PirateHostCommandsTest.PlayCard(PirateHost, PirateMessage)
        }
        [PexMethod]
        public void NewRound(PirateHost host) {
            PirateHostCommands.NewRound(host);
            // TODO: add assertions to method PirateHostCommandsTest.NewRound(PirateHost)
        }
        [PexMethod]
        public void NewPile(PirateHost host) {
            PirateHostCommands.NewPile(host);
            // TODO: add assertions to method PirateHostCommandsTest.NewPile(PirateHost)
        }
        [PexMethod]
        public void KnockKnock(PirateHost host, PirateClient pclient) {
            PirateHostCommands.KnockKnock(host, pclient);
            // TODO: add assertions to method PirateHostCommandsTest.KnockKnock(PirateHost, PirateClient)
        }
        [PexMethod]
        public void InitConnection(
            PirateHost host,
            PirateClient pclient,
            PirateMessage data
        ) {
            PirateHostCommands.InitConnection(host, pclient, data);
            // TODO: add assertions to method PirateHostCommandsTest.InitConnection(PirateHost, PirateClient, PirateMessage)
        }
        [PexMethod]
        public void GetPlayerInfo(PirateHost host, PirateClient pclient) {
            PirateHostCommands.GetPlayerInfo(host, pclient);
            // TODO: add assertions to method PirateHostCommandsTest.GetPlayerInfo(PirateHost, PirateClient)
        }
        [PexMethod]
        public void GameFinished(PirateHost host) {
            PirateHostCommands.GameFinished(host);
            // TODO: add assertions to method PirateHostCommandsTest.GameFinished(PirateHost)
        }
        [PexMethod]
        public void ErrorMessage(
            PirateHost host,
            PirateClient pclient,
            PirateError error
        ) {
            PirateHostCommands.ErrorMessage(host, pclient, error);
            // TODO: add assertions to method PirateHostCommandsTest.ErrorMessage(PirateHost, PirateClient, PirateError)
        }
        [PexMethod]
        public void DealCard(PirateHost host, PirateMessage data) {
            PirateHostCommands.DealCard(host, data);
            // TODO: add assertions to method PirateHostCommandsTest.DealCard(PirateHost, PirateMessage)
        }
        [PexMethod]
        public void BeginRound(PirateHost host) {
            PirateHostCommands.BeginRound(host);
            // TODO: add assertions to method PirateHostCommandsTest.BeginRound(PirateHost)
        }
    }
}
