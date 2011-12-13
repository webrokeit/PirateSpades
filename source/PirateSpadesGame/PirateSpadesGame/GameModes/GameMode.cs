namespace PirateSpadesGame.GameModes {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public interface IGameMode {
        void LoadContent(ContentManager contentManager);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
