﻿using NUnit.Framework;
using System.Collections.Generic;
using PirateSpades.GameLogic;

namespace PirateSpadesTest {
    using System;

    [TestFixture]
    public class TestGame {
        [Test]
        public void SimulateStart() {
            var g = new Game();
            Assert.That(!g.IsFinished());
            Assert.That(!g.IsStarted);
            Assert.That(g.Players == 0);
            var p = new Player("Alberto");
            var p2 = new Player("Carsten");
            var p3 = new Player("Alejandro");
            g.AddPlayer(p);
            Assert.That(g.Players == 1);
            g.AddPlayer(p2);
            Assert.That(g.Players == 2);
            g.AddPlayer(p3);
            Assert.That(g.Players == 3);
            Assert.That(g.Points(p) == 0);
            Assert.That(g.Points(p2) == 0);
            Assert.That(g.Points(p3) == 0);
            Assert.That(g.RoundsPlayed == 0);
            Assert.That(g.Rounds == 20);
            Assert.That(g.RoundsLeft() == 20);
            Table t = Table.GetTable();
            var players = new List<Player>() {p, p2, p3};
            int roundNumber = 1;
            int deal = 10;
            int roundsPlayed = 0;
            while(roundsPlayed != g.Rounds) {
                RotateDealer(players);
                var r = new Round(players, deal, roundNumber);
                r.CollectBet(p, 3);
                r.CollectBet(p2, 3);
                r.CollectBet(p3, 3);
                r.Start();
                for(int i = 0; i < deal; i++) {
                    p = t.PlayerTurn;
                    if(!p.Playable(p.Hand(i))) {
                        int j = 0;
                        while(!p.Playable(p.Hand(j))) {
                            //DO NOTHING
                            j++;
                        }
                        p.PlayCard(p.Hand(j));
                    } else {
                        p.PlayCard(p.Hand(i));
                    }
                    p2 = t.PlayerTurn;
                    if(!p2.Playable(p.Hand(i))) {
                        int k = 0;
                        while(!p2.Playable(p.Hand(k))) {
                            //DO NOTHING
                            k++;
                        }
                        p2.PlayCard(p2.Hand(k));
                    } else {
                        //p2.PlayCard(p2.Hand(i));
                    }
                    p3 = t.PlayerTurn;
                    if(!p3.Playable(p.Hand(i))) {
                        int h = 0;
                        while(!p3.Playable(p.Hand(h))) {
                            //DO NOTHING
                            h++;
                        }
                        p3.PlayCard(p.Hand(h));
                    } else {
                        p3.PlayCard(p3.Hand(i));
                    }
                }
                g.ReceiveStats(r);
                roundNumber++;
                roundsPlayed++;
                if(roundsPlayed < 11) {
                    deal--;
                } else if(roundsPlayed == 11) {
                    //DO NOTHING
                } else {
                    deal++;
                }
            }
        }

        //SAME AS THE ROTATEDEALER METHOD IN GAME
        private void RotateDealer(List<Player> dealership) {
            Player p = dealership[0];
            p.IsDealer = true;
            dealership.Remove(p);
            dealership.Add(p);
        }
    }
}
