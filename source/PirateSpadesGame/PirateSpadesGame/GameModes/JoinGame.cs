
namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using PirateSpades.Network;

    public class JoinGame : IGameMode {
        private PsGame game;
        private bool joinedGame = false;
        private JoinedGame inJoinedGame;
        private Texture2D testTex;
        private Button xButton;
        private Button refreshButton;
        private Button joinGameButton;
        private Button back;
        private Sprite backGround;
        private double frametime;
        private bool mpressed = false;
        private bool prevmpressed = false;
        private Rectangle nameSize;
        private Rectangle ipSize;
        private Rectangle playersSize;
        private SpriteFont font;
        private Rectangle serversRectangle;
        private List<Rectangle> rects;
        private List<ServerSprite> servers;
        private PirateScanner scanner;
        private ContentManager content;

        public JoinGame(PsGame game) {
            servers = new List<ServerSprite>();
            this.game = game;
            this.SetUp(game.Window);
            content = game.Content;
            scanner = new PirateScanner();
            this.Refresh();
        }

        private void SetUp(GameWindow window) {
            backGround = new Sprite() { Color = Color.White };
            var x = window.ClientBounds.Width / 2 - 800 / 2;
            var y = window.ClientBounds.Height / 2 - 500 / 2;
            backGround.Position = new Vector2(x, y);

            var serversX = x + 75;
            var serversY = y + 75;

            serversRectangle = new Rectangle(serversX, serversY, 650, 350);
            this.SetUpServers();


            xButton = new Button("X", x + 650, window.ClientBounds.Height / 2 - 500 / 2 - 10);
            var buttonx = x + 650;
            var buttony = window.ClientBounds.Height / 2 + 200;
            back = new Button("backjoingame", buttonx, buttony);
            buttonx -= Button.Width;
            refreshButton = new Button("refresh", buttonx, buttony);
            buttonx -= Button.Width;
            joinGameButton = new Button("jointhisgame", buttonx, buttony);
        }

        public void SetUpServers() {
            var width = serversRectangle.Width / 3;
            var height = 30;
            nameSize = new Rectangle(serversRectangle.X, serversRectangle.Y, width, height);
            var x = serversRectangle.X + width;
            playersSize = new Rectangle(x, serversRectangle.Y, width, height);
            x += width;
            ipSize = new Rectangle(x, serversRectangle.Y, width, height);
        }

        public void LoadContent(ContentManager contentManager) {
            backGround.LoadContent(contentManager, "findgame");
            xButton.LoadContent(contentManager);
            back.LoadContent(contentManager);
            refreshButton.LoadContent(contentManager);
            joinGameButton.LoadContent(contentManager);
            font = contentManager.Load<SpriteFont>("font");
        }

        public void Update(GameTime gameTime) {
            if(!joinedGame) {
                frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;

                MouseState mouseState = Mouse.GetState();
                int mx = mouseState.X;
                int my = mouseState.Y;
                prevmpressed = mpressed;
                mpressed = mouseState.LeftButton == ButtonState.Pressed;
                this.UpdateButton(xButton, mx, my);
                this.UpdateButton(back, mx, my);
                this.UpdateButton(refreshButton, mx, my);
                this.UpdateButton(joinGameButton, mx, my);
                if(servers.Count > 0) {
                    foreach(var s in servers) {
                        s.Update(gameTime);
                        if(s.DoubleClick) {
                            inJoinedGame = new JoinedGame(false, s.IP);
                            inJoinedGame.LoadContent(content);
                            joinedGame = true;
                        }
                    }
                }
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
                case "jointhisgame":
                    this.JoinSelectedGame();
                    break;
                case "refresh":
                    this.Refresh();
                    break;
                case "backjoingame":
                    game.State = GameState.StartUp;
                    break;
                case "X":
                    game.State = GameState.StartUp;
                    break;
            }
        }

        private void Refresh() {
            var ips = scanner.ScanForIps(4939, 15000);
            foreach(var ip in ips) {
                servers.Add(new ServerSprite(ip, "hej", 1, 5, serversRectangle));
            }
        }

        private void JoinSelectedGame() {
            foreach(var s in servers.Where(s => s.Selected)) {
                inJoinedGame = new JoinedGame(false, s.IP);
                inJoinedGame.LoadContent(content);
                joinedGame = true;
                return;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            if(!joinedGame) {
                backGround.Draw(spriteBatch);
                xButton.Draw(spriteBatch);
                back.Draw(spriteBatch);
                refreshButton.Draw(spriteBatch);
                joinGameButton.Draw(spriteBatch);
                spriteBatch.DrawString(font, "Server Name", new Vector2(nameSize.X, nameSize.Y), Color.Black);
                spriteBatch.DrawString(font, "Players / Max", new Vector2(playersSize.X, playersSize.Y), Color.Black);
                spriteBatch.DrawString(font, "Ip Address", new Vector2(ipSize.X, ipSize.Y), Color.Black);
                if(servers.Count > 0) {
                    foreach(var s in servers) {
                        s.Draw(spriteBatch);
                    }
                } else {
                    spriteBatch.DrawString(
                        font,
                        "No servers available right now",
                        new Vector2(serversRectangle.X, serversRectangle.Y + 30),
                        Color.Black);
                }
            } else {
                inJoinedGame.Draw(spriteBatch);
            }
        }
    }
}
