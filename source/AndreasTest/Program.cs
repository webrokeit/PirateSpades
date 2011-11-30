using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PirateSpades.Network;

namespace AndreasTest {
    using PirateSpades.GameLogic;
    using PirateSpades.Func;

    class Program {
        static void Main(string[] args) {
            try {
                PirateHost host = new PirateHost(4939);
                host.Start();

                PirateClient pc = new PirateClient("Andreas", "localhost", 4939);
                PirateClient pc2 = new PirateClient("Morten", "localhost", 4939);




                while(!host.PlayersReady) {
                    //Console.WriteLine("Waiting for players");
                }

                List<Player> l = new List<Player> { pc, pc2 };

                Game g = new Game();
                g.AddPlayer(pc);
                g.AddPlayer(pc2);

                Table t = Table.GetTable();

                Round r = new Round(l, 5, 6);

                r.Start();

                r.CollectBet(pc, 2);
                r.CollectBet(pc2, 3);
                
                t.Start(g);

                while(pc.NumberOfCards > 0) {
                    Player first = t.PlayerTurn == pc ? pc : pc2;
                    Player second = first == pc ? pc2 : pc;

                    for(var i = 0; i < first.NumberOfCards; i++) {
                        if (first.Playable(first.Hand(i))) {
                            first.PlayCard(first.Hand(i));
                            break;
                        }
                    }

                    for(var i = 0; i < second.NumberOfCards; i++) {
                        if (second.Playable(second.Hand(i))) {
                            second.PlayCard(second.Hand(i));
                            break;
                        }
                    }
                }

                g.ReceiveStats(r);
                t.Stop();

                Console.WriteLine("Andreas Points = " + g.Points(pc));
                Console.WriteLine("Morten Points = " + g.Points(pc2));


            } catch(Exception ex) {
                Console.WriteLine(ex);
            } finally {
                Console.ReadLine();
            }
        }
    }
}
