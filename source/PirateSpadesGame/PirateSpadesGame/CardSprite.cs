using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PirateSpadesGame {
    using PirateSpades.GameLogicV2;

    public class CardSprite : Sprite {
        private Card card;
        private Vector2 table;
        private double ClickTimer;
        private double TimerDelay = 500;

        public CardSprite(Card card, Vector2 position) {
            this.card = card;
            this.Position = position;
            table = new Vector2(0,0);
        }

        public string Name {
            get { return card.Suit + "_" + card.Value; }
        }

        public void LoadContent(ContentManager theContentManager) {
            base.LoadContent(theContentManager, Name);
        }

        public void Update(GameTime theGameTime) {
            MouseState currentMouseState = Mouse.GetState();
            this.UpdateMovement(theGameTime, currentMouseState);
        }

        private void UpdateMovement(GameTime theGameTime, MouseState state) {
            if(state.LeftButton == ButtonState.Pressed) {
                //if(Player.Playable(card) && Player.HaveCard(card)) {
                //    Player.PlayCard(card);
                //    Position = table;
                //}
                Position = table;
            }
        }
    }
}
