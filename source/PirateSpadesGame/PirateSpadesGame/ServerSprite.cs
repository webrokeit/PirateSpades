
namespace PirateSpadesGame {
    using System.Collections.Generic;
    using System.Net;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using PirateSpadesGame.GameModes;

    public class ServerSprite {
        private Rectangle rect;
        private Rectangle ipSize;
        private Rectangle nameSize;
        private Rectangle playersSize;
        private Texture2D tex;
        private int maxPlayers;
        private int players;
        private IPAddress ip;
        private string name;
        private SpriteFont font;
        private int serverNumber;
        private bool mousepressed = false;
        private bool prevmousepressed = false;
        private Rectangle wholeServer;
        private double frametime;
        private double time;

        public ServerSprite(IPAddress ip, string name, int players, int maxPlayers, Rectangle rect, int serverNumber) {
            this.ip = ip;
            this.name = name;
            this.players = players;
            this.maxPlayers = maxPlayers;
            this.rect = rect;
            this.serverNumber = serverNumber;
            Selected = false;
            DoubleClick = false;
            Color = Color.White;
            Timer = 0.0;
            State = BState.Up;
            this.SetUp();
        }

        public BState State { get; private set; }

        public double Timer { get; set; }

        private void SetUp() {
            var width = rect.Width / 3;
            var height = 30;
            var y = rect.Y + height * serverNumber;
            wholeServer = new Rectangle(rect.X, y, rect.Width, height);
            nameSize = new Rectangle(rect.X, y, width, height);
            var x = rect.X + width;
            playersSize = new Rectangle(x, y, width, height);
            x += width;
            ipSize = new Rectangle(x, y, width, height);
        }

        public IPAddress Ip { get { return ip; } }

        public string ServerName { get { return name; } }

        public int MaxPlayers { get { return maxPlayers; } }

        public bool DoubleClick { get; set; }

        public bool Selected { get; set; }

        public Color Color { get; set; }

        public void LoadContent(ContentManager contentManager) {
            tex = contentManager.Load<Texture2D>("servertex");
            font = contentManager.Load<SpriteFont>("font");
        }

        public void Update(GameTime gameTime) {
            var state = Mouse.GetState();
            frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;
            this.UpdateMovement(state);
        }

        private void UpdateMovement(MouseState state) {
            int mx = state.X;
            int my = state.Y;
            prevmousepressed = mousepressed;
            mousepressed = state.LeftButton == ButtonState.Pressed;

            if(HitAlpha(wholeServer, tex, mx, my)) {
                Timer = 0.0;
                if(mousepressed) {
                    State = BState.Down;
                } else if(!mousepressed && prevmousepressed && State == BState.Down) {
                    State = BState.JustReleased;
                } else {
                    State = BState.Hover;
                    Color = Color.LightBlue;
                }
            } else {
                State = BState.Up;
                if(Timer > 0) {
                    Timer = Timer - frametime;
                    Color = Color.DarkBlue;
                } else {
                    Color = Color.Transparent;
                }
            }
            if(State == BState.JustReleased) {
                bool wasSelected = Selected;
                lock(JoinGame.Serversprites) {
                    foreach (var s in JoinGame.Serversprites) {
                        s.Selected = false;
                    }
                }
                if(wasSelected) {
                    DoubleClick = true;
                } else {
                    Color = Color.Green;
                    Selected = true;
                }
            }
        }

        public bool HitAlpha(Rectangle rectangle, Texture2D texture, int x, int y) {
            return HitAlpha(0, 0, texture, texture.Width * (x - rectangle.X) /
                rectangle.Width, texture.Height * (y - rectangle.Y) / rectangle.Height);
        }

        private bool HitAlpha(float tx, float ty, Texture2D texture, int x, int y) {
            if (this.Hit(tx, ty, texture, x, y)) {
                var data = new uint[texture.Width * texture.Height];
                texture.GetData<uint>(data);
                if ((x - (int)tx) + (y - (int)ty) * texture.Width < texture.Width * texture.Height) {
                    return ((data[(x - (int)tx) + (y - (int)ty) * texture.Width] & 0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }

        private bool Hit(float tx, float ty, Texture2D texture, int x, int y) {
            return (x >= tx &&
                x <= tx + texture.Width &&
                y >= ty &&
                y <= ty + texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(this.tex, this.wholeServer, this.Selected ? Color.Turquoise : this.Color);
            spriteBatch.DrawString(font, name, new Vector2(nameSize.X, nameSize.Y), Color.White);
            spriteBatch.DrawString(font, this.players + "/"+this.maxPlayers, new Vector2(playersSize.X, playersSize.Y), Color.White);
            spriteBatch.DrawString(font, ip.ToString(), new Vector2(ipSize.X, ipSize.Y), Color.White);
        }
    }
}
