// <copyright file="Button.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      Class used for making buttons
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>

﻿namespace PirateSpadesGame.Misc {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    using System.Diagnostics.Contracts;

    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Class used for making buttons
    /// </summary>
    public class Button {
        private readonly string name;
        private Texture2D buttonTexture;
        private const int height = 50;
        private const int width = 150;
        private double frametime;
        private bool mpressed = false;
        private bool prevmpressed = false;


        /// <summary>
        /// The constructor for button takes a string and two integers
        /// </summary>
        /// <param name="name">The string (name) is used to specify what texture to load</param>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Button(string name, int x, int y) {
            Contract.Requires(name != null);
            this.name = name;
            Timer = 0.0;
            State = BState.Up;
            Color = Color.White;
            Rectangle = new Rectangle(x, y, Width, Height);
        }

        /// <summary>
        /// The name of the Button
        /// </summary>
        public string Name { get { Contract.Ensures(name != null); return name; } }

        /// <summary>
        /// The width of the button
        /// </summary>
        public static int Width { get {
            Contract.Ensures(Contract.Result<int>() > 0); return width; } }

        /// <summary>
        /// The height of the button
        /// </summary>
        public static int Height { get { Contract.Ensures(Contract.Result<int>() > 0); return height; } }

        /// <summary>
        /// A helper timer to determine clicks
        /// </summary>
        public double Timer { get; set; }

        /// <summary>
        /// The button's texture
        /// </summary>
        public Texture2D Tex { get { Contract.Ensures(Contract.Result<Texture2D>() != null); return buttonTexture; } }

        /// <summary>
        /// The state of the button
        /// </summary>
        public BState State { get; set; }

        /// <summary>
        /// The button's color
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The button's rectangle
        /// </summary>
        public Rectangle Rectangle { get; set; }

        /// <summary>
        /// Load the content for the button
        /// </summary>
        /// <param name="contentManager">The ContentManager used to load the content</param>
        public void LoadContent(ContentManager contentManager) {
            buttonTexture = contentManager.Load<Texture2D>(Name);
        }

        /// <summary>
        /// Draw the button on the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(buttonTexture, Rectangle, Color);
        }

        /// <summary>
        /// Update the button
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <returns>True if the button has been pressed and released</returns>
        public bool Update(GameTime gameTime) {
            Contract.Ensures(Contract.Result<bool>() == (State == BState.JustReleased));
            frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;
            MouseState mouseState = Mouse.GetState();
            int mx = mouseState.X;
            int my = mouseState.Y;
            prevmpressed = mpressed;
            mpressed = mouseState.LeftButton == ButtonState.Pressed && PsGame.Active && mouseState.X >= 0 && mouseState.X < PsGame.Width && mouseState.Y >= 0 && mouseState.Y < PsGame.Height;

            if(Func.HitAlpha(Rectangle, Tex, mx, my)) {
                Timer = 0.0;
                if(mpressed) {
                    State = BState.Down;
                    Color = Color.GhostWhite;
                    return false;
                }
                if(!this.mpressed && this.prevmpressed && this.State == BState.Down) {
                    this.State = BState.JustReleased;
                } else {
                    this.State = BState.Hover;
                    this.Color = Color.White;
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
     /// <summary>
     /// Class used for describing a buttons state
     /// </summary>
    public enum BState {
        Hover, Up, JustReleased, Down
    }
}
