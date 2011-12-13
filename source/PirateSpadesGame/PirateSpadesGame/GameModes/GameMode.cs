// <copyright file="GameMode.cs">
//      mche@itu.dk, hclk@itu.dk
// </copyright>
// <summary>
//      Class used for drawing the playing cards
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>
// <author>Helena Charlotte Lyn Krüger(hclk@itu.dk)</author>

﻿namespace PirateSpadesGame.GameModes {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An interface describing a GameMode
    /// </summary>
    public interface IGameMode {
        void LoadContent(ContentManager contentManager);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
