// <copyright file="CreateGame.cs">
//      mche@itu.dk, hclk@itu.dk
// </copyright>
// <summary>
//      Class used for making create game screen
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>
// <author>Helena Charlotte Lyn Krüger (hclk@itu.dk)</author>

namespace PirateSpadesGame.GameModes {
    using System.Diagnostics.Contracts;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.Linq;
    using PirateSpades.Network;

    using PirateSpadesGame.Misc;

    using Game = PirateSpades.GameLogic.Game;

    /// <summary>
    /// Class used for making create game screen
    /// </summary>
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

        /// <summary>
        /// The constructor for CreateGame takes a PsGame
        /// </summary>
        /// <param name="game">The currently running PsGame</param>
        public CreateGame(PsGame game) {
            Contract.Requires(game != null);
            this.game = game;
            this.SetUp();
        }

        /// <summary>
        /// Set up the create game screen
        /// </summary>
        private void SetUp() {
            backGround = new Sprite { Color = Color.White };
            var x = game.Window.ClientBounds.Width / 2 - 400 / 2;
            var y = game.Window.ClientBounds.Height / 2 - 400 / 2;
            backGround.Position = new Vector2(x, y);

            var cgX = x;
            var cgY = y + 325;
            createGame = new Button("creategame", cgX, cgY);

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

        /// <summary>
        /// Load the content for this create game screen
        /// </summary>
        /// <param name="contentManager">The ContentManager to load the content</param>
        public void LoadContent(ContentManager contentManager) {
            backGround.LoadContent(contentManager, "creategamewindow");
            font = contentManager.Load<SpriteFont>("font");
            cancel.LoadContent(contentManager);
            createGame.LoadContent(contentManager);
            serverName.LoadContent(contentManager);
            numberOfPlayers.LoadContent(contentManager);
        }

        /// <summary>
        /// Update this create game screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            foreach(var b in this.buttons.Where(b => b.Update(gameTime))) {
                this.ButtonAction(b);
            }
            numberOfPlayers.Update(gameTime);
            serverName.Update(gameTime);
        }

        /// <summary>
        /// Helper method for taking action upon a button press
        /// </summary>
        /// <param name="b">The button that has been pressed</param>
        private void ButtonAction(Button b) {
            Contract.Requires(b != null);
            Contract.Ensures((PirateHost.IsValidGameName(serverName.Text) ? (game.Host != null && game.Client != null && game.PlayingGame != null && game.State == GameState.InGame) : game.State == GameState.CreateGame)
                || game.State == GameState.StartUp);
            var str = b.Name;
            switch(str) {
                case "creategame":
                    if(serverName.Text == "" || !PirateHost.IsValidGameName(serverName.Text)) {
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

        /// <summary>
        /// Draw this create game screen on the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            backGround.Draw(spriteBatch);
            cancel.Draw(spriteBatch);
            serverName.Draw(spriteBatch);
            numberOfPlayers.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Server Name:", namePos, Color.White);
            spriteBatch.DrawString(font, "Max Players:", playersPos, Color.White);
            createGame.Draw(spriteBatch);
        }

    }
}