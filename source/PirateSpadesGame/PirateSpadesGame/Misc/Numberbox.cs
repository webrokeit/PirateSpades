// <copyright file="Numberbox.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      Class used for making boxes that only takes number input
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>

﻿namespace PirateSpadesGame.Misc {
     using System.Diagnostics.Contracts;

     using Microsoft.Xna.Framework;
     using Microsoft.Xna.Framework.Content;
     using Microsoft.Xna.Framework.Graphics;
     using Microsoft.Xna.Framework.Input;

     using System;
     using System.Linq;

     /// <summary>
     /// Class used for making boxes that only takes number input
     /// </summary>
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

         /// <summary>
         /// The constructor for numberbox takes a rectangle, a string and a integer
         /// </summary>
         /// <param name="rect">The rectangle is used to specify the size of the numberbox</param>
         /// <param name="name">The string (name) is used to specify what texture to load</param>
         /// <param name="maxNumber">The integer (maxNumber) is used to specify how many numbers may be inputted (Example: 3 if 999 is the Limit)</param>
         public Numberbox(Rectangle rect, string name, int maxNumber) {
             Contract.Requires(name != null && maxNumber > 0);
             box = rect;
             this.name = name;
             this.maxNumber = maxNumber;
         }

         /// <summary>
         /// How big can the number of the number be?
         /// Set the Limit of the numberbox
         /// </summary>
         public int Limit { get; set; }

         /// <summary>
         /// What number does the numberbox specify
         /// </summary>
         public int Number { get; set; }

         /// <summary>
         /// What string is the numberbox representing
         /// </summary>
         public string Text { get; set; }

         /// <summary>
         /// Load the content of the numberbox
         /// </summary>
         /// <param name="contentManager">The ContentManager used to load the content</param>
         public void LoadContent(ContentManager contentManager) {
             font = contentManager.Load<SpriteFont>("font");
             tex = contentManager.Load<Texture2D>(name);
         }

         /// <summary>
         /// Update the numberbox
         /// </summary>
         /// <param name="gameTime">Provides a snapshot of timing values.</param>
         public void Update(GameTime gameTime) {
             MouseState state = Mouse.GetState();
             if(state.LeftButton == ButtonState.Pressed && PsGame.Active && state.X >= 0 && state.X < PsGame.Width && state.Y >= 0 && state.Y < PsGame.Height) {
                 typable = true;
             }
             if(state.LeftButton == ButtonState.Pressed && (state.X < box.X || state.X > (box.X + box.Width) || state.Y < box.Y || state.Y > (box.Y + box.Height)) && (PsGame.Active && state.X >= 0 && state.X < PsGame.Width && state.Y >= 0 && state.Y < PsGame.Height)) {
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

         /// <summary>
         /// Checks if the specified key has been pressed and released
         /// </summary>
         /// <param name="k">
         /// The key to check
         /// </param>
         /// <returns>
         /// True if the key has been pressed and released
         /// </returns>
         private bool CheckKey(Keys k) {
             Contract.Ensures(Contract.Result<bool>() == (this.lastKeyboardState.IsKeyDown(k) && this.currentKeyboardState.IsKeyUp(k)));
             return lastKeyboardState.IsKeyDown(k) && currentKeyboardState.IsKeyUp(k);
         }

         /// <summary>
         /// Parse the number the numberbox is representing to float
         /// </summary>
         /// <returns>The float</returns>
         public float ParseInputToFloat() {
             if(Text.Length == 0) {
                 return 0f;
             }
             if(Number > Limit) {
                 Number = Limit;
                 Text = Number.ToString();
             }
             return Number / (float)Limit;
         }

         /// <summary>
         /// Parse the number the numberbox is representing to int
         /// </summary>
         /// <returns></returns>
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

         /// <summary>
         /// Add a number to the numberbox or remove a number from the numberbox
         /// </summary>
         /// <param name="k">The key</param>
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

         /// <summary>
         /// Draw the numberbox on the given SpriteBatch
         /// </summary>
         /// <param name="spriteBatch">The SpriteBatch</param>
         public void Draw(SpriteBatch spriteBatch) {
             spriteBatch.Draw(tex, box, Color.White);
             spriteBatch.DrawString(font, Text, new Vector2(box.X + 20, box.Y + box.Height / 2 - 10), Color.Black);
         }
     }
 }
