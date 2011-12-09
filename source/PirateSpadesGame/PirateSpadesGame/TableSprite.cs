using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PirateSpadesGame {
    public class TableSprite : Sprite {
        //private Table table = Table.GetTable();
        private List<CardSprite> cards = new List<CardSprite>(); 

        public string Name { get { return "Table"; } }

        public void LoadContent(ContentManager theContentManager) {
            base.LoadContent(theContentManager, Name);
        }

        /*public void Update(GameTime theGameTime) {
            if(table.Cards > 0) {
                foreach(var c in table.PlayedCards) {
                    new CardSprite(c, new Vector2());
                }
            }
            if(table.Cards == 0) {
                cards.Clear();
            }
        }*/
    }
}
