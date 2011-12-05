
namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class StartUp : IGameMode {
        private Sprite title;
        private List<Button> buttons;
        private const int numberOfButtons = 5;
        private bool mpressed = false;
        private bool prevmpressed = false;
        private double frametime;
        private bool settingsEnabled;
        private bool rulesEnabled = false;
        private Sprite rules;
        private Button back;

        public StartUp(GameWindow window) {
            this.SetUp(window);
        }

        private void SetUp(GameWindow window) {
            title = new Sprite { Color = PsGame.Color };
            var x = window.ClientBounds.Width / 2 - 200;
            title.Position = new Vector2(x, 0);

            rules = new Sprite() { Color = Color.White };
            int rulesX = window.ClientBounds.Width / 2 - 450 / 2;
            int rulesY = window.ClientBounds.Height / 2 - 588 / 2;
            rules.Position = new Vector2(rulesX, rulesY);
            int backX = (rulesX + 450) / 2 + Button.Width / 2;
            int backY = (rulesY + 515);
            back = new Button("back", backX, backY);

            x = window.ClientBounds.Width / 2 - Button.Width / 2;
            var y = window.ClientBounds.Height / 2 - numberOfButtons / 2 * Button.Height - (numberOfButtons % 2) * Button.Height / 2;

            buttons = new List<Button>();
            buttons.Add(new Button("joingame", x, y));
            y += Button.Height;
            buttons.Add(new Button("creategame", x, y));
            y += Button.Height;
            buttons.Add(new Button("rules", x, y));
            y += Button.Height;
            buttons.Add(new Button("settings", x, y));
            y += Button.Height;
            buttons.Add(new Button("exit", x, y));
        }

        public void LoadContent(ContentManager contentManager) {
            title.LoadContent(contentManager, "pspades");
            rules.LoadContent(contentManager, "Gamerules");
            back.LoadContent(contentManager);
            foreach(var b in buttons) {
                b.LoadContent(contentManager);
            }
        }

        public void Update(GameTime gameTime) {
            frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;

            MouseState mouseState = Mouse.GetState();
            int mx = mouseState.X;
            int my = mouseState.Y;
            prevmpressed = mpressed;
            mpressed = mouseState.LeftButton == ButtonState.Pressed;
            foreach(var b in buttons) {
                UpdateButton(b, mx, my);
            }
            if(rulesEnabled) {
                this.UpdateButton(back, mx, my);
            }
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
                case "joingame":
                    PsGame.Color = Color.Aquamarine;
                    break;
                case "creategame":
                    PsGame.Color = Color.Beige;
                    break;
                case "rules":
                    rulesEnabled = true;
                    break;
                case "settings":
                    PsGame.Color = Color.Yellow;
                    break;
                case "back":
                    rulesEnabled = false;
                    break;
                case "exit":
                    PsGame.State = GameState.Exit;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            title.Color = PsGame.Color;
            title.Draw(spriteBatch);
            foreach(var b in buttons) {
                b.Draw(spriteBatch);
            }
            if(rulesEnabled) {
                rules.Draw(spriteBatch);
                back.Draw(spriteBatch);
            }
        }
    }
}
