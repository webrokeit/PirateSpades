
namespace PirateSpadesGame.GameModes {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class JoinGame : IGameMode {
        private PsGame game;
        private bool joinedGame;
        private JoinedGame inJoinedGame;
        private Texture2D testTex;
        private Button back;
        private Sprite backGround;
        private double frametime;
        private bool mpressed = false;
        private bool prevmpressed = false;

        public JoinGame(PsGame game) {
            this.game = game;
            back = new Button("ok", 100, 100);
            this.SetUp(game.Window);
        }

        private void SetUp(GameWindow window) {
            
        }

        public void LoadContent(ContentManager contentManager) {
            backGround.LoadContent(contentManager, "findgame");
            testTex = contentManager.Load<Texture2D>("Gamerules");
            back.LoadContent(contentManager);
        }

        public void Update(GameTime gameTime) {
            frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;

            MouseState mouseState = Mouse.GetState();
            int mx = mouseState.X;
            int my = mouseState.Y;
            prevmpressed = mpressed;
            mpressed = mouseState.LeftButton == ButtonState.Pressed;
            this.UpdateButton(back, mx, my);
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
                case "ok":
                    game.State = GameState.StartUp;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(testTex, new Rectangle(0, 0, testTex.Width, testTex.Height), Color.White);
            back.Draw(spriteBatch);
        }
    }
}
