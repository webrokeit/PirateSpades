//Helena
namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class InGame : IGameMode {
        private Vector2 screenCenter;
        private Dictionary<string, int> score;

        public InGame() {

            Dictionary <int, Dictionary<string, int>>;
        }

        protected void LoadContent()
        {
        }

        public void LoadContent(ContentManager contentManager) {
        }

        public void Update(GameTime gameTime) {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Tab)) {
                score.Add();
            }

        }

        public void Draw(SpriteBatch spriteBatch) {
            throw new System.NotImplementedException();
        }

        public void draw(SpriteBatch spritebatch){}

      
    }
}
