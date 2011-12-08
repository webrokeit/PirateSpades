using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpadesGame.GameModes {
    using System.Net;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class CreateGame : IGameMode {
        private PsGame game;
        private Sprite backGround;
        private JoinedGame inJoinedGame;
        private bool joinedGame = false;
        private Button cancel;
        private Button createGame;
        private bool mpressed;
        private bool prevmpressed;
        private double frametime;
        private Numberbox numberOfPlayers;
        private Textbox serverName;
        private Vector2 namePos;
        private Vector2 playersPos;
        private SpriteFont font;
        private ContentManager content;

        public CreateGame(PsGame game) {
            this.game = game;
            this.content = game.Content;
            this.SetUp(game.Window);
        }

        private void SetUp(GameWindow window) {
            backGround = new Sprite() { Color = Color.White };
            var x = window.ClientBounds.Width / 2 - 400 / 2;
            var y = window.ClientBounds.Height / 2 - 400 /2;
            backGround.Position = new Vector2(x, y);

            var cgX = x + 10;
            var cgY = y + 325;
            createGame = new Button("creategamegm", cgX, cgY);

            var cancelX = x + 285;
            var cancelY = y+ 325;
            cancel = new Button("cancelcg", cancelX, cancelY);

            var rect = new Rectangle(x+250, y+200, 100, 50);
            numberOfPlayers = new Numberbox(rect, "volumebox", 1) { Limit = 5, Number = 5 };
            numberOfPlayers.Text = numberOfPlayers.Number.ToString();

            var sRect = new Rectangle(x+150, y+100, 250, 75);
            serverName = new Textbox(sRect, "playername");
            serverName.MoveText(45);

            namePos = new Vector2(x + 10, y + 125);
            playersPos = new Vector2(x+10, y+225);
        }

        public void LoadContent(ContentManager contentManager) {
            backGround.LoadContent(contentManager, "creategamewindow");
            font = contentManager.Load<SpriteFont>("font");
            cancel.LoadContent(contentManager);
            createGame.LoadContent(contentManager);
            serverName.LoadContent(contentManager);
            numberOfPlayers.LoadContent(contentManager);
        }

        public void Update(GameTime gameTime) {
            if(!joinedGame) {
                frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;

                MouseState mouseState = Mouse.GetState();
                int mx = mouseState.X;
                int my = mouseState.Y;
                prevmpressed = mpressed;
                mpressed = mouseState.LeftButton == ButtonState.Pressed;

                this.UpdateButton(cancel, mx, my);
                this.UpdateButton(createGame, mx, my);
                numberOfPlayers.Update(gameTime);
                serverName.Update(gameTime);
            } else {
                inJoinedGame.Update(gameTime);
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
                case "creategamegm":
                    if(serverName.Text == "") {
                        return;
                    }
                    var players = numberOfPlayers.ParseInput();
                    var sName = serverName.Text;
                    //inJoinedGame = new JoinedGame(true, IPAddress.Any);
                    //inJoinedGame.LoadContent(content);
                    //joinedGame = true;
                    break;
                case "cancelcg":
                    game.State = GameState.StartUp;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            if(!joinedGame) {
                backGround.Draw(spriteBatch);
                cancel.Draw(spriteBatch);
                createGame.Draw(spriteBatch);
                serverName.Draw(spriteBatch);
                numberOfPlayers.Draw(spriteBatch);
                spriteBatch.DrawString(font, "Server Name:", namePos, Color.White);
                spriteBatch.DrawString(font, "Max Players:", playersPos, Color.White);
            } else {
                inJoinedGame.Draw(spriteBatch);
            }
        }
    }
}
