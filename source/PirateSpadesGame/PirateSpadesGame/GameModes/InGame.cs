//Helena
namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using PirateSpades.Misc;
    using PirateSpades.GameLogicV2;
    using PirateSpades.Network;

    using Game = PirateSpades.GameLogicV2.Game;

    public class InGame : IGameMode {
        private Vector2 screenCenter;
        private Dictionary<string, Dictionary<string, int>> score;
        private bool playing = false;
        private bool showMenu = false;
        private PirateClient client;
        private PirateHost host;
        private Game playingGame;
        private bool hosting;
        private Sprite menu;
        private Button leaveGame;
        private Button startGame;
        private Button exitGame;
        private Button resumeGame;
        private Button menuleaveGame;
        private List<Button> menuButtons; 
        private PsGame game;

        public InGame(PsGame game) {
            this.SetUp();
        }

        private void SetUp() {
            if(game.Host == null) {
                hosting = false;
            } else {
                host = game.Host;
                hosting = true;
            }
            client = game.Client;
            playingGame = game.PlayingGame;

            startGame = new Button("startgame", 0, 0);
            leaveGame = new Button("leavegame", 0, 0);

            menu = new Sprite() { Color = Color.White };
            var menuX = 0;
            var menuY = 0;
            menu.Position = new Vector2(menuX, menuY);
            menuleaveGame = new Button("leavegame", 0, 0);
            resumeGame = new Button("resumegame", 0, 0);
            exitGame = new Button("exitgame", 0, 0);
            menuButtons = new List<Button>() { this.menuleaveGame, this.resumeGame, this.exitGame };

        }

        public void LoadContent(ContentManager contentManager) {
            exitGame.LoadContent(contentManager);
            resumeGame.LoadContent(contentManager);
            menuleaveGame.LoadContent(contentManager);
            startGame.LoadContent(contentManager);
            leaveGame.LoadContent(contentManager);
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
                case "startgame":
                    if(host.Game.Players.Count >= Game.MinPlayersInGame && host.Game.Players.Count <= Game.MaxPlayersInGame) {
                        host.Game.Start(true, CollectionFnc.PickRandom(0, host.Game.Players.Count - 1));
                    }
                    break;
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
            if(showMenu) {
                exitGame.Draw(spriteBatch);
                menuleaveGame.Draw(spriteBatch);
                resumeGame.Draw(spriteBatch);
            }
        }

      
    }
}
