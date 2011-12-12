using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PirateSpadesGame {
    using System;
    using System.Linq;

    public class Numberbox {
        private Rectangle box;
        private bool typable = false;
        private readonly Keys[] keysToCheck = new Keys[] {
            Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7,
            Keys.NumPad8, Keys.NumPad9, Keys.Enter, Keys.Back, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
            Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Down, Keys.Up};
        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;
        private SpriteFont font;
        private Texture2D tex;
        private string name;
        private int maxNumber;

        public Numberbox(Rectangle rect, string name, int maxNumber) {
            box = rect;
            this.name = name;
            this.maxNumber = maxNumber;
        }

        public int Limit { get; set; }

        public int Number { get; set; }

        public string Text { get; set; }

        public void LoadContent(ContentManager contentManager) {
            font = contentManager.Load<SpriteFont>("font");
            tex = contentManager.Load<Texture2D>(name);
        }

        public void Update(GameTime gameTime) {
            MouseState state = Mouse.GetState();
            if(state.LeftButton == ButtonState.Pressed) {
                typable = true;
            }
            if(state.LeftButton == ButtonState.Pressed && (state.X < box.X || state.X > (box.X + box.Width) || state.Y < box.Y || state.Y > (box.Y + box.Height))) {
                typable = false;
            }
            
            if(typable) {
                currentKeyboardState = Keyboard.GetState();

                foreach(var k in this.keysToCheck.Where(this.CheckKey)) {
                    this.AddLetter(k);
                    break;
                }
                lastKeyboardState = currentKeyboardState;
            }
        }

        private bool CheckKey(Keys k) {
            return lastKeyboardState.IsKeyDown(k) && currentKeyboardState.IsKeyUp(k);
        }

        public float ParseInputToFloat() {
            if(Text.Length == 0) {
                return 0;
            }
            if(Number > Limit) {
                Number = Limit;
                Text = Number.ToString();
                return (float) Number;
            }
            return Number /(float)Limit;
        }

        public int ParseInput() {
            if(Text.Length == 0) {
                return 0;
            }
            if(Number > Limit) {
                Number = Limit;
                Text = Number.ToString();
                return Limit;
            }
            return Number;
        }

        private void AddLetter(Keys k) {
           var newChar = "";
            
            if(Text.Length == maxNumber && k != Keys.Back && k != Keys.Down)
                return;

            var curNum = 0;
            switch(k) {
                case Keys.Down:
                    if(int.TryParse(Text, out curNum) && curNum > 0) {
                        curNum -= 1;
                    }
                    Text = curNum.ToString();
                    break;
                case Keys.Up:
                    if(int.TryParse(Text, out curNum) && curNum < int.MaxValue) {
                        curNum += 1;
                    }
                    Text = curNum.ToString();
                    break;
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
                case Keys.Enter:
                    typable = false;
                    break;
                case Keys.Back:
                    if(Text.Length != 0) {
                        Text = Text.Remove(Text.Length - 1);
                        if(Text.Length > 0) {
                            Number = Convert.ToInt32(Text);
                        }
                    }
                    return;
            }
            Text += newChar;
            if(Text.Length > 0) {
                Number = Convert.ToInt32(Text);
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(tex, box, Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(box.X + 20, box.Y + box.Height / 2 - 10), Color.Black);
        }
    }
}
