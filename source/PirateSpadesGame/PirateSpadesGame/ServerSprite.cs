
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
        private int maxPlayers;
        private int players;
        private IPAddress ip;
        private string name;
        private SpriteFont font;

        public ServerSprite(IPAddress ip, string name, int players, int maxPlayers, Rectangle rect) {
            this.ip = ip;
            this.name = name;
            this.players = players;
            this.maxPlayers = maxPlayers;
            this.rect = rect;
            Selected = false;
            DoubleClick = false;
            Color = Color.White;
            this.SetUp();
        }

        private void SetUp() {
            var width = rect.Width / 3;
            var height = 30;
            nameSize = new Rectangle(rect.X, rect.Y, width, height);
            var x = rect.X + width;
            playersSize = new Rectangle(x, rect.Y, width, height);
            x += width;
            ipSize = new Rectangle(x, rect.Y, width, height);
        }

        public IPAddress IP { get { return ip; } }

        public bool DoubleClick { get; set; }

        public bool Selected { get; set; }

        public Color Color { get; set; }

        public void LoadContent(ContentManager contentManager) {
            font = contentManager.Load<SpriteFont>("font");
        }

        public void Update(GameTime gameTime) {
            MouseState state = Mouse.GetState();
            this.UpdateMovement(state);
        }

        private void UpdateMovement(MouseState state) {
            if(Selected == true && state.LeftButton == ButtonState.Pressed) {
                DoubleClick = true;
            } else if(state.LeftButton == ButtonState.Pressed) {
                Selected = true;
            }
            if(state.LeftButton == ButtonState.Pressed && (state.X < rect.X || state.X > (rect.X + rect.Width) || state.Y < rect.Y || state.Y > (rect.Y + rect.Height))) {
                Selected = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            this.Color = this.Selected ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, name, new Vector2(nameSize.X, nameSize.Y), Color);
            spriteBatch.DrawString(font, this.players + "/"+this.maxPlayers, new Vector2(playersSize.X, playersSize.Y), Color);
            spriteBatch.DrawString(font, ip.ToString(), new Vector2(ipSize.X, ipSize.Y), Color);
        }
    }
}
