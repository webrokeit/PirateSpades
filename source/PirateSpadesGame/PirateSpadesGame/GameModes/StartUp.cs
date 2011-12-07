
namespace PirateSpadesGame.GameModes {
    using System;
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
        private Sprite settings;
        private PsGame game;
        private Button cancel;
        private Button apply;
        private Textbox playername;
        private string playerName = "Player Name:";
        private Vector2 playerNamePos;
        private SpriteFont font;
        private Numberbox volume;
        private Vector2 volumePos;
        private string volumeString = "Volume:";
        private string scoreboardKey = "Press and hold TAB to show scoreboard \n when ingame";
        private Vector2 scoreboardPos;
        private string menuKey = "Press ESC to show menu when ingame";
        private Vector2 menuPos;

        public StartUp(PsGame game) {
            this.game = game;
            this.SetUp(game.Window);
        }

        private void SetUp(GameWindow window) {
            this.SetUpRules(window);
            this.SetUpSettings(window);

            var x = window.ClientBounds.Width / 2 - Button.Width / 2;
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

        public void SetUpRules(GameWindow window) {
            rules = new Sprite() { Color = Color.White };
            int rulesX = window.ClientBounds.Width / 2 - 450 / 2;
            int rulesY = window.ClientBounds.Height / 2 - 588 / 2;
            rules.Position = new Vector2(rulesX, rulesY);
            int backX = (rulesX + 450) / 2 + Button.Width / 2;
            int backY = (rulesY + 515);
            back = new Button("back", backX, backY);
        }

        private void SetUpSettings(GameWindow window) {
            settings = new Sprite() { Color = Color.White };
            int settingsX = window.ClientBounds.Width / 2 - 600 / 2;
            int settingsY = window.ClientBounds.Height / 2 - 468 / 2;
            settings.Position = new Vector2(settingsX, settingsY);
            int cancelX = settingsX + 425;
            int cancelY = settingsY + 400;
            cancel = new Button("cancel", cancelX, cancelY);
            int applyX = settingsX + 40;
            int applyY = settingsY + 400;
            apply = new Button("apply", applyX, applyY);
            var rect = new Rectangle(settingsX + (600-325), settingsY + 100, 250, 75);
            playername = new Textbox(rect, "playername") { Text = this.game.PlayerName };
            playername.MoveText(45);
            playerNamePos = new Vector2(settingsX+100, settingsY + 125);
            var volumeRect = new Rectangle(settingsX + (600 - 325) + 100, settingsY + 185, 100, 50);
            var a = (int)Math.Round(game.MusicVolume);
            volume = new Numberbox(volumeRect, "volumebox") { Number = a * 100 };
            volume.Text = volume.Number.ToString();
            volumePos = new Vector2(settingsX + 100, settingsY + 200);
            scoreboardPos = new Vector2(settingsX + 100, settingsY + 250);
            menuPos = new Vector2(settingsX + 100, settingsY + 325);
        }

        public void LoadContent(ContentManager contentManager) {
            //title.LoadContent(contentManager, "pspades");
            rules.LoadContent(contentManager, "Gamerules");
            settings.LoadContent(contentManager, "Gamesettings");
            back.LoadContent(contentManager);
            font = contentManager.Load<SpriteFont>("font");
            cancel.LoadContent(contentManager);
            apply.LoadContent(contentManager);
            playername.LoadContent(contentManager);
            volume.LoadContent(contentManager);
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
            if(settingsEnabled) {
                this.UpdateButton(cancel, mx, my);
                this.UpdateButton(apply, mx, my);
                playername.Update(gameTime);
                volume.Update(gameTime);
            } else if(rulesEnabled) {
                this.UpdateButton(back, mx, my);
            } else {
                foreach (var b in buttons) {
                    UpdateButton(b, mx, my);
                }
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
                    game.State = GameState.JoinGame;
                    break;
                case "creategame":
                    game.State = GameState.CreateGame;
                    break;
                case "rules":
                    rulesEnabled = true;
                    break;
                case "settings":
                    playername.Text = this.game.PlayerName;
                    settingsEnabled = true;
                    break;
                case "back":
                    rulesEnabled = false;
                    break;
                case "cancel":
                    CancelChanges();
                    settingsEnabled = false;
                    break;
                case "apply":
                    this.ApplyChanges();
                    settingsEnabled = false;
                    break;
                case "exit":
                    game.State = GameState.Exit;
                    break;
            }
        }

        private void CancelChanges() {
            playername.Text = game.PlayerName;
            var a = (int)Math.Round(game.MusicVolume);
            volume.Number = a * 100;
            volume.Text = volume.Number.ToString();
        }

        private void ApplyChanges() {
            game.PlayerName = playername.Text;
            game.MusicVolume = volume.ParseInput();
        }

        public void Draw(SpriteBatch spriteBatch) {
            //title.Draw(spriteBatch);
            foreach(var b in buttons) {
                b.Draw(spriteBatch);
            }
            if(rulesEnabled) {
                rules.Draw(spriteBatch);
                back.Draw(spriteBatch);
            }
            if(settingsEnabled) {
                settings.Draw(spriteBatch);
                apply.Draw(spriteBatch);
                cancel.Draw(spriteBatch);
                playername.Draw(spriteBatch);
                volume.Draw(spriteBatch);
                spriteBatch.DrawString(font, playerName, playerNamePos, Color.Black);
                spriteBatch.DrawString(font, volumeString, volumePos, Color.Black);
                spriteBatch.DrawString(font, scoreboardKey, scoreboardPos, Color.Black);
                spriteBatch.DrawString(font, menuKey, menuPos, Color.Black);
            }
        }
    }
}
