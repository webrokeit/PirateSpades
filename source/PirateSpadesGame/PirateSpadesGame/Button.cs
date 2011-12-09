using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PirateSpadesGame {
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// the buttons in the menu is defined in this class
    /// </summary>
    public class Button {
        private readonly string name;
        private Texture2D buttonTexture;
        private const int height = 50;
        private const int width = 150;
        private double frametime;
        private bool mpressed = false;
        private bool prevmpressed = false;

        public Button(string name, int x, int y) {
            this.name = name;
            Timer = 0.0;
            State = BState.Up;
            Color = Color.White;
            Rectangle = new Rectangle(x, y, Width, Height);
        }

        public string Name { get { return name; } }

        public static int Width { get { return width; } }

        public static int Height { get { return height; } }

        public double Timer { get; set; }

        public Texture2D Tex { get { return buttonTexture; } }

        public BState State { get; set; }

        public Color Color { get; set; }

        public Rectangle Rectangle { get; set; }

        public void LoadContent(ContentManager contentManager) {
            buttonTexture = contentManager.Load<Texture2D>(Name);
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(buttonTexture, Rectangle, Color);
        }

        public bool Update(GameTime gameTime) {
            frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;
            MouseState mouseState = Mouse.GetState();
            int mx = mouseState.X;
            int my = mouseState.Y;
            prevmpressed = mpressed;
            mpressed = mouseState.LeftButton == ButtonState.Pressed;

            if(HitAlpha(Rectangle, Tex, mx, my)) {
                Timer = 0.0;
                if(mpressed) {
                    State = BState.Down;
                    Color = Color.GhostWhite;
                    return false;
                } else if(!mpressed && prevmpressed && State == BState.Down) {
                    State = BState.JustReleased;
                } else {
                    State = BState.Hover;
                    Color = Color.White;
                    return false;
                }
            } else {
                State = BState.Up;
                if(Timer > 0) {
                    Timer = Timer - frametime;
                    return false;
                } else {
                    Color = Color.CornflowerBlue;
                    return false;
                }
            }
            if(State == BState.JustReleased) {
                return true;
            }
            return false;
        }

        public bool HitAlpha(Rectangle rect, Texture2D tex, int x, int y) {
            return HitAlpha(0, 0, tex, tex.Width * (x - rect.X) /
                rect.Width, tex.Height * (y - rect.Y) / rect.Height);
        }

        private bool HitAlpha(float tx, float ty, Texture2D tex, int x, int y) {
            if (this.Hit(tx, ty, tex, x, y)) {
                var data = new uint[tex.Width * tex.Height];
                tex.GetData<uint>(data);
                if ((x - (int)tx) + (y - (int)ty) * tex.Width < tex.Width * tex.Height) {
                    return ((data[(x - (int)tx) + (y - (int)ty) * tex.Width] & 0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }

        private bool Hit(float tx, float ty, Texture2D tex, int x, int y) {
            return (x >= tx &&
                x <= tx + tex.Width &&
                y >= ty &&
                y <= ty + tex.Height);
        }
    }

    public enum BState {
        Hover, Up, JustReleased, Down
    }
}
