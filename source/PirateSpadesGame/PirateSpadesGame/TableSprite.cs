//Helena, Morten 
using System.Collections.Generic;
using PirateSpades.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PirateSpadesGame {
    using Microsoft.Xna.Framework.Graphics;

    public class TableSprite : Sprite {
        private Table table = Table.GetTable();
        private List<CardSprite> cards = new List<CardSprite>();
        private GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        private ContentManager content;
        private float acc = 0f;
        private SpriteBatch spriteBatch;

        protected void LoadContent() {
            spriteBatch = new SpriteBatch(graphicsDevice);
            /// TODO: ship billed skal indsættes i content
            var ship = this.content.Load<Texture2D>("ship");
        }

        public string Name { get { return "Table"; } }

        public void LoadContent(ContentManager theContentManager) {
            base.LoadContent(theContentManager, Name);
        }

        public void Update(GameTime gameTime) {
            if (table.Cards > 0) {
                foreach (var c in table.PlayedCards) {
                    new CardSprite(c, new Vector2());
                }
            }
            if (table.Cards == 0) {
                cards.Clear();
            }

        }
        

        public void Draw(SpriteBatch spriteBatch) {
            graphics.GraphicsDevice.Clear(Color.Pink);
            spriteBatch.Begin();

            /// TODO: fill out
            spriteBatch.Begin();

            spriteBatch.End();

            base.Draw(spriteBatch);
        }
    }
}
