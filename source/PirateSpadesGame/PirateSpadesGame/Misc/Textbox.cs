namespace PirateSpadesGame.Misc {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using System.Diagnostics.Contracts;
    using System.Linq;

    public class Textbox {
        private Rectangle box;
        private readonly Keys[] keysToCheck = new Keys[] {
		Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
		Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
		Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
		Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
		Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
		Keys.Z, Keys.Back, Keys.Space, Keys.Enter, 
        Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, 
        Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9 };
        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;
        private SpriteFont font;
        private Texture2D tex;
        private string name;
        private int textmove;

        public Textbox(Rectangle rect, string name) {
            Contract.Requires(name != null);
            Text = "";
            box = rect;
            this.name = name;
            textmove = 0;
            Typable = false;
        }

        public bool Typable { get; set; }

        public string Text { get; set; }

        public void LoadContent(ContentManager contentManager) {
            font = contentManager.Load<SpriteFont>("font");
            tex = contentManager.Load<Texture2D>(name);
        }

        public void MoveText(int a) {
            Contract.Requires(a > 0);
            textmove += a;
        }

        public void Update(GameTime gameTime) {
            MouseState state = Mouse.GetState();
            if(state.LeftButton == ButtonState.Pressed && PsGame.Active && state.X >= 0 && state.X < PsGame.Width && state.Y >= 0 && state.Y < PsGame.Height) {
                Typable = true;
            }
            if(state.LeftButton == ButtonState.Pressed && (state.X < box.X || state.X > (box.X + box.Width) || state.Y < box.Y || state.Y > (box.Y + box.Height)) && (PsGame.Active && state.X >= 0 && state.X < PsGame.Width && state.Y >= 0 && state.Y < PsGame.Height)) {
                Typable = false;
            }

            if(Typable) {
                currentKeyboardState = Keyboard.GetState();
                foreach(var k in this.keysToCheck.Where(this.CheckKey)) {
                    this.AddLetter(k);
                    break;
                }
                lastKeyboardState = currentKeyboardState;
            }
        }

        private bool CheckKey(Keys k) {
            Contract.Ensures(Contract.Result<bool>() == (this.lastKeyboardState.IsKeyDown(k) && this.currentKeyboardState.IsKeyUp(k)));
            return lastKeyboardState.IsKeyDown(k) && currentKeyboardState.IsKeyUp(k);
        }

        private void AddLetter(Keys k) {
            var newChar = "";
            string str = "E";

            if(font.MeasureString(Text + str).Length() + textmove > box.Width && k != Keys.Back)
                return;

            switch(k) {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.NumPad1:
                case Keys.D1:
                    newChar += "1";
                    break;
                case Keys.NumPad2:
                case Keys.D2:
                    newChar += "2";
                    break;
                case Keys.NumPad3:
                case Keys.D3:
                    newChar += "3";
                    break;
                case Keys.NumPad4:
                case Keys.D4:
                    newChar += "4";
                    break;
                case Keys.NumPad5:
                case Keys.D5:
                    newChar += "5";
                    break;
                case Keys.NumPad6:
                case Keys.D6:
                    newChar += "6";
                    break;
                case Keys.NumPad7:
                case Keys.D7:
                    newChar += "7";
                    break;
                case Keys.NumPad8:
                case Keys.D8:
                    newChar += "8";
                    break;
                case Keys.NumPad9:
                case Keys.D9:
                    newChar += "9";
                    break;
                case Keys.NumPad0:
                case Keys.D0:
                    newChar += "0";
                    break;
                case Keys.Space:
                    newChar += " ";
                    break;
                case Keys.Enter:
                    Typable = false;
                    break;
                case Keys.Back:
                    if(Text.Length != 0)
                        Text = Text.Remove(Text.Length - 1);
                    return;
            }
            if(currentKeyboardState.IsKeyDown(Keys.RightShift) ||
                currentKeyboardState.IsKeyDown(Keys.LeftShift)) {
                newChar = newChar.ToUpper();
            }
            Text += newChar;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(tex, box, Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(box.X + textmove, box.Y + box.Height / 2 - 10), Color.Black);
        }
    }
}
