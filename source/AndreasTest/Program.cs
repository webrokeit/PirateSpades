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
                pc.IsDealer = true;

                while(!host.PlayersReady) {
                    //Console.WriteLine("Waiting for players");
                }

                pc.DealCards(new List<Player> { pc }, 5);

            } catch(Exception ex) {
                Console.WriteLine(ex);
            } finally {
                Console.ReadLine();
            }
        }
    }
}
