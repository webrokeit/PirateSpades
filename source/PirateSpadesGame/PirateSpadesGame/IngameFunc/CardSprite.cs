// <copyright file="CardSprite.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      Class used for drawing the playing cards
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>

namespace PirateSpadesGame.IngameFunc {
    using System.Diagnostics.Contracts;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Input;
    using System;
    using Microsoft.Xna.Framework.Graphics;
    using PirateSpades.GameLogic;

    using PirateSpadesGame.Misc;

    /// <summary>
    /// Class used for drawing the playing cards
    /// </summary>
    public class CardSprite {
        private Texture2D cardSprite;
        private double timer;
        private bool mousepressed = false;
        private bool prevmousepressed = false;
        private double frametime;

        /// <summary>
        /// The constructor for CardSprite takes a card and a rectangle
        /// </summary>
        /// <param name="card">The card to be grapically represented</param>
        /// <param name="rect">The rectangle is used to specify the size of the graphical card</param>
        public CardSprite(Card card, Rectangle rect) {
            Contract.Requires(card != null);
            this.Card = card;
            this.Rect = rect;
            Color = Color.White;
        }

        /// <summary>
        /// The name of card "Suit_Value" (Example: Spades_Ace)
        /// </summary>
        public string Name {
            get { return this.Card.Suit + "_" + this.Card.Value; }
        }

        /// <summary>
        /// The state of the card
        /// </summary>
        public BState State { get; private set; }

        /// <summary>
        /// The color of the cardsprite
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Has this cardsprite been double clicked?
        /// </summary>
        public bool DoubleClick { get { return clickCount >= 1 && (DateTime.Now - lastClick).TotalSeconds <= 1.0; }}

        /// <summary>
        /// What card does this cardsprite represent?
        /// </summary>
        public Card Card { get; private set; }

        /// <summary>
        /// What is the rectangle used to specify the size of this cardsprite?
        /// </summary>
        public Rectangle Rect { get; private set; }

        private int clickCount = 0;

        private DateTime lastClick = DateTime.Now;

        /// <summary>
        /// Load the content of this cardsprite
        /// </summary>
        /// <param name="theContentManager">The ContentManager used to load the content</param>
        public void LoadContent(ContentManager theContentManager) {
            cardSprite = theContentManager.Load<Texture2D>(Name);
        }

        /// <summary>
        /// Update this cardsprite
        /// </summary>
        /// <param name="theGameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime theGameTime) {
            var currentMouseState = Mouse.GetState();
            this.frametime = theGameTime.ElapsedGameTime.Milliseconds / 1000.0;
            this.UpdateMovement(currentMouseState);
        }

        /// <summary>
        /// Helper method for updating the cardsprite
        /// </summary>
        /// <param name="state">The current mouse state</param>
        private void UpdateMovement(MouseState state) {
            var mx = state.X;
            var my = state.Y;
            prevmousepressed = mousepressed;
            mousepressed = state.LeftButton == ButtonState.Pressed && PsGame.Active && state.X >= 0 && state.X < PsGame.Width && state.Y >= 0 && state.Y < PsGame.Height;

            if(Func.HitAlpha(Rect, cardSprite, mx, my)) {
                timer = 0.0;
                if(mousepressed) {
                    State = BState.Down;
                } else if(!mousepressed && prevmousepressed && State == BState.Down) {
                    State = BState.JustReleased;
                } else {
                    State = BState.Hover;
                }
            } else {
                State = BState.Up;
                if(timer > 0) {
                    timer = timer - frametime;
                } else {
                    Color = Color.White;
                }
            }
            if(State == BState.JustReleased) {
                var curClick = DateTime.Now;
                if((curClick - lastClick).TotalSeconds <= 1.0) {
                    clickCount++;
                }else {
                    clickCount = 0;
                }
                lastClick = curClick;
            }
        }

        /// <summary>
        /// Draw this cardsprite on the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(this.cardSprite, this.Rect, Color.White);
        }
    }
}
