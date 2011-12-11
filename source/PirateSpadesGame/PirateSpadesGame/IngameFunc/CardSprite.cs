using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace PirateSpadesGame {
    using Microsoft.Xna.Framework.Graphics;

    using PirateSpades.GameLogic;

    using PirateSpadesGame.GameModes;

    public class CardSprite {
        private Texture2D cardSprite;
        private double timer;
        private bool mousepressed = false;
        private bool prevmousepressed = false;
        private double frametime;

        public CardSprite(Card card, Rectangle rect) {
            this.Card = card;
            this.Rect = rect;
            Scale = 1.0f;
            Color = Color.White;
        }

        public string Name {
            get { return this.Card.Suit + "_" + this.Card.Value; }
        }

        public Vector2 Position { get; set; }

        public BState State { get; private set; }

        public bool Selected { get; set; }

        public Color Color { get; set; }

        public bool DoubleClick { get; set; }

        public Card Card { get; private set; }

        public Rectangle Rect { get; private set; }

        public float Scale { get; set; }

        public void LoadContent(ContentManager theContentManager) {
            cardSprite = theContentManager.Load<Texture2D>(Name);
        }

        public void Update(GameTime theGameTime) {
            var currentMouseState = Mouse.GetState();
            this.frametime = theGameTime.ElapsedGameTime.Milliseconds / 1000.0;
            this.UpdateMovement(currentMouseState);
        }

        private void UpdateMovement(MouseState state) {
            var mx = state.X;
            var my = state.Y;
            prevmousepressed = mousepressed;
            mousepressed = state.LeftButton == ButtonState.Pressed;

            if(HitAlpha(Rect, cardSprite, mx, my)) {
                timer = 0.0;
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
                if(timer > 0) {
                    timer = timer - frametime;
                } else {
                    Color = Color.White;
                }
            }
            if(State == BState.JustReleased) {
                var wasSelected = Selected;
                lock(InGame.Cardsprites) {
                    foreach(var c in InGame.Cardsprites) {
                        c.Selected = false;
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
            if(this.Hit(tx, ty, texture, x, y)) {
                var data = new uint[texture.Width * texture.Height];
                texture.GetData<uint>(data);
                if((x - (int)tx) + (y - (int)ty) * texture.Width < texture.Width * texture.Height) {
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
            spriteBatch.Draw(this.cardSprite, this.Rect, Color.White);
        }
    }
}
