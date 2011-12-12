using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PirateSpadesGame {
    public interface IGameMode {
        void LoadContent(ContentManager contentManager);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
