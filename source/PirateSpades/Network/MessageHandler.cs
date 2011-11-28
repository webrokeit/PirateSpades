using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpades.Network {
    public class MessageHandler {
        public static void ReceivedMessage(PirateClient pclient, PirateMessage msg) {
            if(msg.Head == PirateMessageHead.Xcrd) {
                
            }
        } 
    }
}
