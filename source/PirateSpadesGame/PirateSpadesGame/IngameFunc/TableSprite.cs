
namespace PirateSpadesGame.IngameFunc {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
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
        private Player winner;
        private DateTime roundOver;
        private SpriteFont font;
        private int ourIndex;

        private static readonly Dictionary<int, List<Vector2>> CardPlacements = new Dictionary<int, List<Vector2>>() {
            { 2, new List<Vector2> { new Vector2(425, 400), new Vector2(425, 305) } },
            { 3, new List<Vector2> { new Vector2(425, 400), new Vector2(295, 330), new Vector2(545, 340) } },
            {
                4,
                new List<Vector2>
                { new Vector2(425, 400), new Vector2(145, 385), new Vector2(425, 305), new Vector2(685, 400) }
                },
            {
                5,
                new List<Vector2> {
                    new Vector2(425, 400),
                    new Vector2(145, 385),
                    new Vector2(295, 330),
                    new Vector2(545, 340),
                    new Vector2(685, 400)
                }
                }
        };

        public TableSprite(PsGame game, Game playingGame, Rectangle rect) {
            Contract.Requires(game != null && playingGame != null);
            this.game = game;
            this.playingGame = playingGame;
            this.rect = rect;
            this.SetUp();
        }

        private void SetUp() {
            cardsOnTable = new List<CardSprite>();
            playingGame.RoundFinished += OnRoundFinished;
            playingGame.RoundNewPile += OnRoundFinished;
            ourIndex = playingGame.PlayerIndex(game.Client);
        }

        public void LoadContent(ContentManager contentManager) {
            playingGround = contentManager.Load<Texture2D>(playingGame.Players.Count + "_player");
            font = contentManager.Load<SpriteFont>("font2");
        }

        public void Update(GameTime gameTime) {
            if(playingGame.Started && playingGame.Round.BoardCards.Pile.Count > 0) {
                var cards = CardPlacements[playingGame.Players.Count];
                cardsOnTable.Clear();
                foreach(var p in playingGame.Round.BoardCards.Pile.Keys) {
                    var curIndex = playingGame.PlayerIndex(p);
                    var tmpIndex = curIndex - ourIndex;
                    var realIndex = (playingGame.Players.Count + tmpIndex) % playingGame.Players.Count;

                    var c = playingGame.Round.BoardCards.Pile[p];
                    var cs = new CardSprite(c, new Rectangle((int)cards[realIndex].X, (int)cards[realIndex].Y, 50, 60));
                    cs.LoadContent(game.Content);
                    cardsOnTable.Add(cs);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(playingGround, rect, Color.White);
            if(cardsOnTable.Count > 0) {
                foreach(var cs in cardsOnTable) {
                    cs.Draw(spriteBatch);
                }
            }
            if(winner != null && (DateTime.Now - roundOver).TotalSeconds <= 4.0) {
                var str = winner.Name + " won the trick";
                var t = font.MeasureString(str);
                var x = game.Window.ClientBounds.Width / 2f - t.X;
                var y = game.Window.ClientBounds.Height / 2f - t.Y + 20;
                spriteBatch.DrawString(font,
                    str,
                    new Vector2(x, y),
                    Color.Blue);
            }
            
        }

        private void OnRoundFinished(Game g) {
            Contract.Ensures(cardsOnTable.Count == 0);
            cardsOnTable.Clear();
            winner = g.Round.LastTrick.Winner;
            roundOver = DateTime.Now;
        }
    }
}
