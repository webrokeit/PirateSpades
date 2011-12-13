//Helena
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PirateSpadesGame {
    /// <summary>
    /// Interface which ChreateGame, InGame, JoinGame and StartUp implements.
    /// </summary>
    public interface IGameMode {
        void LoadContent(ContentManager contentManager);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
