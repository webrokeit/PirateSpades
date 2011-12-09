
namespace PirateSpadesGame {
    using System.Collections.Generic;
    using System.Net;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

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
            this.SetUp();
        }

        private void SetUp() {
            var width = rect.Width / 3;
            var height = 30;
            var y = rect.Y + height * serverNumber;
            nameSize = new Rectangle(rect.X, y, width, height);
            var x = rect.X + width;
            playersSize = new Rectangle(x, y, width, height);
            x += width;
            ipSize = new Rectangle(x, y, width, height);
        }

        public IPAddress IP { get { return ip; } }

        public bool DoubleClick { get; set; }

        public bool Selected { get; set; }

        public Color Color { get; set; }

        public void LoadContent(ContentManager contentManager) {
            tex = contentManager.Load<Texture2D>("servertex");
            font = contentManager.Load<SpriteFont>("font");
        }

        public void Update(GameTime gameTime) {
            var state = Mouse.GetState();
            this.UpdateMovement(state);
        }

        private void UpdateMovement(MouseState state) {
            mousepressed = state.LeftButton == ButtonState.Pressed;
            if(mousepressed && (state.X < rect.X || state.X > (rect.X + rect.Width) || state.Y < rect.Y || state.Y > (rect.Y + rect.Height))) {
                Selected = false;
            }
            if(mousepressed) {
                Selected = true;
            } else if(Selected && mousepressed) {
                DoubleClick = true;
            }
            prevmousepressed = mousepressed;
        }

        public void Draw(SpriteBatch spriteBatch) {
            this.Color = this.Selected ? Color.Yellow : Color.White;
            spriteBatch.Draw(tex, new Rectangle(nameSize.X, nameSize.Y, 650, 30), Color);
            spriteBatch.DrawString(font, name, new Vector2(nameSize.X, nameSize.Y), Color.White);
            spriteBatch.DrawString(font, this.players + "/"+this.maxPlayers, new Vector2(playersSize.X, playersSize.Y), Color.White);
            spriteBatch.DrawString(font, ip.ToString(), new Vector2(ipSize.X, ipSize.Y), Color.White);
        }
    }
}
