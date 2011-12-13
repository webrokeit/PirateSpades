// <copyright file="Sprite.cs">
//      mche@itu.dk, hclk@itu.dk
// </copyright>
// <summary>
//      Helper class used for drawing and loading textures
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>
// <author>Helena Charlotte Lyn Krüger (hclk@itu.dk) </author>

﻿namespace PirateSpadesGame.Misc {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Helper class for drawting textures.
    /// </summary>
    public class Sprite {
        public Vector2 Position = new Vector2(0,0);
        private Texture2D mSpriteTexture;
        public Rectangle Size;
        public float Scale = 1.0f;

        /// <summary>
        /// Constructor for the class Sprite.
        /// </summary>
        public Sprite() {
            Color = Color.White;
        }

        /// <summary>
        /// Change or get the color of the texture.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Get the texture this class represents
        /// </summary>
        public Texture2D Tex { get { return mSpriteTexture; } }

        /// <summary>
        /// Load the content for this sprite with this contentmanager and this file
        /// </summary>
        /// <param name="theContentManager">
        /// The ContentManager that should be used to load the texture
        /// </param>
        /// <param name="theAssetName">
        /// The name of the file to be load (as it is written in the Content)
        /// </param>
        public void LoadContent(ContentManager theContentManager, string theAssetName) {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            Size = new Rectangle(0, 0, (int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale));
        }

        /// <summary>
        /// Draw the Sprite on this texture
        /// </summary>
        /// <param name="theSpriteBatch">
        /// The spritebatch which the texture will be drawn on
        /// </param>
        public void Draw(SpriteBatch theSpriteBatch) {
            theSpriteBatch.Draw(mSpriteTexture, Position,
                new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height),
                Color, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
