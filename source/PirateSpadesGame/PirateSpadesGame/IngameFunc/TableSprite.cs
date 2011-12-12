
namespace PirateSpadesGame.IngameFunc {
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using PirateSpades.Misc;
    using PirateSpades.GameLogic;
    using PirateSpades.Network;

    using Game = PirateSpades.GameLogic.Game;

    public class TableSprite {
        private Game playingGame;
        private PsGame game;
        private Texture2D playingGround;
        private List<CardSprite> cardsOnTable;
        private Rectangle rect;
 

        public TableSprite(PsGame game, Game playingGame, Rectangle rect) {
            this.game = game;
            this.playingGame = playingGame;
            this.rect = rect;
            this.SetUp();
        }

        private void SetUp() {
            cardsOnTable = new List<CardSprite>();
            playingGame.RoundFinished += OnRoundFinished;
        }

        public void LoadContent(ContentManager contentManager) {
            playingGround = contentManager.Load<Texture2D>(playingGame.Players.Count + "_player");
        }

        public void Update(GameTime gameTime) {
            var tempX = 400;
            foreach(var c in playingGame.Round.BoardCards.Pile.Values) {
                var cs = new CardSprite(c, new Rectangle(tempX,300,50,60));
                cs.LoadContent(game.Content);
                cardsOnTable.Add(cs);
                tempX += 50;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(playingGround, rect, Color.White);
            if(cardsOnTable.Count > 0) {
                foreach(var cs in cardsOnTable) {
                    cs.Draw(spriteBatch);
                }
            }
        }

        private void OnRoundFinished(Game g) {
            cardsOnTable.Clear();
        }
    }
}
