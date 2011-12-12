// Helena
namespace PirateSpadesGame.GameModes {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.Linq;
    using PirateSpades.Network;
    using Game = PirateSpades.GameLogic.Game;

    public class CreateGame : IGameMode {
        private PsGame game;
        private Sprite backGround;
        private Button cancel;
        private Button createGame;
        private Numberbox numberOfPlayers;
        private Textbox serverName;
        private Vector2 namePos;
        private Vector2 playersPos;
        private SpriteFont font;

        private List<Button> buttons;

        public CreateGame(PsGame game) {
            this.game = game;
            this.SetUp(game.Window);
        }

        private void SetUp(GameWindow window) {
            backGround = new Sprite { Color = Color.White };
            var x = window.ClientBounds.Width / 2 - 400 / 2;
            var y = window.ClientBounds.Height / 2 - 400 / 2;
            backGround.Position = new Vector2(x, y);

            var cgX = x;
            var cgY = y + 325;
            createGame = new Button("creategamegm", cgX, cgY);

            var cancelX = x + 250;
            var cancelY = y + 325;
            cancel = new Button("cancelcg", cancelX, cancelY);

            var rect = new Rectangle(x + 250, y + 200, 100, 50);
            numberOfPlayers = new Numberbox(rect, "volumebox", 1) { Limit = 5, Number = 5 };
            numberOfPlayers.Text = numberOfPlayers.Number.ToString();

            var sRect = new Rectangle(x + 150, y + 100, 250, 75);
            serverName = new Textbox(sRect, "playername");
            serverName.MoveText(45);

            namePos = new Vector2(x + 10, y + 125);
            playersPos = new Vector2(x + 10, y + 225);

            buttons = new List<Button> { this.cancel, this.createGame };
        }

        public void LoadContent(ContentManager contentManager) {
            backGround.LoadContent(contentManager, "creategamewindow");
            font = contentManager.Load<SpriteFont>("font");
            cancel.LoadContent(contentManager);
            createGame.LoadContent(contentManager);
            serverName.LoadContent(contentManager);
            numberOfPlayers.LoadContent(contentManager);
        }

        public void Update(GameTime gameTime) {
            foreach(var b in this.buttons.Where(b => b.Update(gameTime))) {
                this.ButtonAction(b);
            }
            numberOfPlayers.Update(gameTime);
            serverName.Update(gameTime);
        }

        private void ButtonAction(Button b) {
            if(b == null) {
                return;
            }
            var str = b.Name;
            switch(str) {
                case "creategamegm":
                    if(serverName.Text == "" || PirateHost.IsValidGameName(serverName.Text)) {
                        return;
                    }
                    var players = numberOfPlayers.ParseInput();
                    var sName = serverName.Text;
                    var host = new PirateHost(4939);
                    host.Start(sName, players);
                    var client = new PirateClient(game.PlayerName, host.Ip, 4939);
                    PirateClientCommands.SendPlayerInfo(client);
                    var playingGame = new Game();
                    game.GameName = sName;
                    game.MaxPlayers = players;
                    game.Host = host;
                    game.Client = client;
                    client.SetGame(playingGame);
                    game.PlayingGame = playingGame;
                    game.State = GameState.InGame;
                    break;
                case "cancelcg":
                    game.State = GameState.StartUp;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            backGround.Draw(spriteBatch);
            cancel.Draw(spriteBatch);
            createGame.Draw(spriteBatch);
            serverName.Draw(spriteBatch);
            numberOfPlayers.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Server Name:", namePos, Color.White);
            spriteBatch.DrawString(font, "Max Players:", playersPos, Color.White);
        }

    }
}