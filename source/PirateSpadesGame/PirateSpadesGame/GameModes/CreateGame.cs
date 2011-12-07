using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpadesGame.GameModes {
    using System.Net;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class CreateGame : IGameMode {
        private PsGame game;
        private Sprite backGround;
        private JoinedGame inJoinedGame;
        private bool joinedGame;
        private Button cancel;
        private Button createGame;
        private bool mpressed;
        private bool prevmpressed;
        private double frametime;

        public CreateGame(PsGame game) {
            this.game = game;
            this.SetUp(game.Window);
        }

        private void SetUp(GameWindow window) {
            backGround = new Sprite() { Color = Color.White };
            var x = window.ClientBounds.Width / 2;
            var y = window.ClientBounds.Height / 2;
            backGround.Position = new Vector2(x, y);

            var cgX = 0;
            var cgY = 0;
            createGame = new Button("creategamegm", cgX, cgY);

            var cancelX = 0;
            var cancelY = 0;
            cancel = new Button("cancelcg", cancelX, cancelY);
        }

        public void LoadContent(ContentManager contentManager) {
            cancel.LoadContent(contentManager);
            createGame.LoadContent(contentManager);
        }

        public void Update(GameTime gameTime) {
            
        }

        private void UpdateButton(Button b, int mx, int my) {
            if(b.HitAlpha(b.Rectangle, b.Tex, mx, my)) {
                b.Timer = 0.0;
                if(mpressed) {
                    b.State = BState.Down;
                    b.Color = Color.GhostWhite;
                } else if(!mpressed && prevmpressed && b.State == BState.Down) {
                    b.State = BState.JustReleased;
                } else {
                    b.State = BState.Hover;
                    b.Color = Color.White;
                }
            } else {
                b.State = BState.Up;
                if(b.Timer > 0) {
                    b.Timer = b.Timer - frametime;
                } else {
                    b.Color = Color.CornflowerBlue;
                }
            }
            if(b.State == BState.JustReleased) {
                ButtonAction(b);
            }

        }

        private void ButtonAction(Button b) {
            if(b == null) {
                return;
            }
            var str = b.Name;
            switch(str) {
                case "creategamegm":
                    inJoinedGame = new JoinedGame(true, IPAddress.Any);
                    break;
                case "cancelcg":
                    game.State = GameState.StartUp;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            
        }
    }
}
