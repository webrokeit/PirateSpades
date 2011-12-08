using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpadesGame.GameModes {
    using System.Net;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class JoinedGame : IGameMode {
        private bool host;

        public JoinedGame(bool host, IPAddress ip) {
            
        }

        public void LoadContent(ContentManager contentManager) {
            //Load button "Start game" if host is true
        }

        public void Update(GameTime gameTime) {
            
        }

        public void Draw(SpriteBatch spriteBatch) {
            
        }
    }
}
