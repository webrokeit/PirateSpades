//Helena
namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class InGame : IGameMode {
        private Vector2 screenCenter;
        private Dictionary<string, Dictionary<string, int>> score;
        private bool playing = false;
        private bool showMenu = false;
        private Sprite Menu;
        private Button leaveGame;
        private Button startGame;
        private Button exitGame;
        private Button resumeGame;
        private List<Button> menuButtons; 
        private PsGame game;

        public InGame(PsGame game) {
            this.SetUp();
        }

        private void SetUp() {
            startGame = new Button("startgame", 0, 0);


            leaveGame = new Button("leavegame", 0, 0);
            resumeGame = new Button("resumegame", 0, 0);
            exitGame = new Button("exitgame", 0, 0);
            menuButtons = new List<Button>() { this.leaveGame, this.resumeGame, this.exitGame };

        }

        public void LoadContent(ContentManager contentManager) {
            exitGame.LoadContent(contentManager);
            resumeGame.LoadContent(contentManager);
            leaveGame.LoadContent(contentManager);
            startGame.LoadContent(contentManager);
        }

        public void Update(GameTime gameTime) {
            if(showMenu) {
                foreach (var b in this.menuButtons.Where(b => b.Update(gameTime))) {
                    this.ButtonAction(b);
                }
            }
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Tab)) {
                //TODO: do this;
            }

        }

        private void ButtonAction(Button b) {
            if(b == null) {
                return;
            }
            var str = b.Name;
            switch(str) {
                case "resumegame":
                    showMenu = false;
                    break;
                case "leavegame":
                    game.State = GameState.StartUp;
                    break;
                case "exitgame":
                    game.State = GameState.Exit;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            throw new System.NotImplementedException();
        }

        public void draw(SpriteBatch spritebatch){}

      
    }
}
