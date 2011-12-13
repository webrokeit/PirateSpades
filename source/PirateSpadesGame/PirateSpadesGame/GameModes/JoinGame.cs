// <copyright file="JoinGame.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      Class used for making the join game screen
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>

namespace PirateSpadesGame.GameModes {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using PirateSpades.Network;

    using PirateSpadesGame.Misc;

    using Game = PirateSpades.GameLogic.Game;

    public class JoinGame : IGameMode {
        private PsGame game;
        private Button xButton;
        private Button refreshButton;
        private Button joinGameButton;
        private Button back;
        private Sprite backGround;
        private Rectangle nameSize;
        private Rectangle ipSize;
        private Rectangle playersSize;
        private SpriteFont font;
        private Rectangle serversRectangle;
        private List<ServerSprite> servers;
        private PirateScanner scanner;
        private ContentManager content;
        public static IList<ServerSprite> Serversprites = new List<ServerSprite>().AsReadOnly();
        private List<Button> buttons;
        private int numberOfServers;
        private bool refreshing = false;
        private delegate IList<PirateScanner.GameInfo> ScanForGamesDelegate(int port, int timeout);

        /// <summary>
        /// The constructor for JoinGame takes a PsGame
        /// </summary>
        /// <param name="game">The currently running PsGame</param>
        public JoinGame(PsGame game) {
            Contract.Requires(game != null);
            servers = new List<ServerSprite>();
            this.game = game;
            this.SetUp();
            content = game.Content;
            scanner = new PirateScanner();
            this.Refresh();
        }

        /// <summary>
        /// Set up the screen
        /// </summary>
        private void SetUp() {
            backGround = new Sprite() { Color = Color.White };
            var x = game.Window.ClientBounds.Width / 2 - 800 / 2;
            var y = game.Window.ClientBounds.Height / 2 - 500 / 2;
            backGround.Position = new Vector2(x, y);

            var serversX = x + 75;
            var serversY = y + 75;

            serversRectangle = new Rectangle(serversX, serversY, 650, 30);
            this.SetUpServers();


            xButton = new Button("X", x + 650, game.Window.ClientBounds.Height / 2 - 500 / 2 - 10);
            var buttonx = x + 650;
            var buttony = game.Window.ClientBounds.Height / 2 + 200;
            back = new Button("backjoingame", buttonx, buttony);
            buttonx -= Button.Width;
            refreshButton = new Button("refresh", buttonx, buttony);
            buttonx -= Button.Width;
            joinGameButton = new Button("jointhisgame", buttonx, buttony);
            buttons = new List<Button> { this.xButton, this.refreshButton, this.joinGameButton, this.back };
        }

        /// <summary>
        /// Set up the servers
        /// </summary>
        private void SetUpServers() {
            var width = serversRectangle.Width / 3;
            var height = 30;
            nameSize = new Rectangle(serversRectangle.X, serversRectangle.Y, width, height);
            var x = serversRectangle.X + width;
            playersSize = new Rectangle(x, serversRectangle.Y, width, height);
            x += width;
            ipSize = new Rectangle(x, serversRectangle.Y, width, height);
        }

        /// <summary>
        /// Load the content of this join game screen
        /// </summary>
        /// <param name="contentManager">The ContentManger used to load the content</param>
        public void LoadContent(ContentManager contentManager) {
            backGround.LoadContent(contentManager, "findgame");
            xButton.LoadContent(contentManager);
            back.LoadContent(contentManager);
            refreshButton.LoadContent(contentManager);
            joinGameButton.LoadContent(contentManager);
            font = contentManager.Load<SpriteFont>("font");
        }

        /// <summary>
        /// Update the join game screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            foreach(var b in this.buttons.Where(b => b.Update(gameTime))) {
                this.ButtonAction(b);
            }
            if(servers.Count > 0) {
                foreach(var s in servers) {
                    s.Update(gameTime);
                    if(s.DoubleClick) {
                        this.JoinThisGame(s);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Helper method for taking action upon a button press
        /// </summary>
        /// <param name="b">The button that has been pressed</param>
        private void ButtonAction(Button b) {
            Contract.Requires(b != null);
            var str = b.Name;
            switch(str) {
                case "jointhisgame":
                    this.JoinSelectedGame();
                    break;
                case "refresh":
                    this.Refresh();
                    break;
                case "backjoingame":
                    game.State = GameState.StartUp;
                    break;
                case "X":
                    game.State = GameState.StartUp;
                    break;
            }
        }

        /// <summary>
        /// Refresh and find new servers
        /// </summary>
        private void Refresh() {
            if(refreshing) return;
            servers.Clear();
            refreshing = true;
            numberOfServers = 0;

            const int Port = 4939;
            const int Timeout = 15000;

            scanner.GameFound += GameFound;
            var del = new ScanForGamesDelegate(scanner.ScanForGames);
            del.BeginInvoke(Port, Timeout, RefreshDone, del);
        }

        /// <summary>
        /// Done refreshing
        /// </summary>
        /// <param name="ar">An asynchronous result</param>
        private void RefreshDone(IAsyncResult ar) {
            var del = (ScanForGamesDelegate)ar.AsyncState;
            var res = del.EndInvoke(ar);

            scanner.GameFound += GameFound;

            refreshing = false;
        }

        /// <summary>
        /// Helper method for the event GameFound
        /// </summary>
        /// <param name="gameInfo">The information about the game</param>
        private void GameFound(PirateScanner.GameInfo gameInfo) {
            lock(servers) {
                numberOfServers++;
                var serverSprite = new ServerSprite(gameInfo.Ip, gameInfo.GameName, gameInfo.Players, gameInfo.MaxPlayers, serversRectangle, numberOfServers);
                serverSprite.LoadContent(content);
                servers.Add(serverSprite);
                Serversprites = servers.AsReadOnly();
            }
        }

        /// <summary>
        /// Join the selected server
        /// </summary>
        private void JoinSelectedGame() {
            foreach(var s in servers.Where(s => s.Selected)) {
                this.JoinThisGame(s);
                return;
            }
        }

        /// <summary>
        /// Join the server represented by this serversprite
        /// </summary>
        /// <param name="s">The serversprite</param>
        private void JoinThisGame(ServerSprite s) {
            Contract.Requires(s != null && Regex.IsMatch(game.PlayerName, @"^[a-zA-Z0-9]{3,12}$"));
            Contract.Ensures(game.Client != null && game.PlayingGame != null && game.Host == null && game.State == GameState.InGame);
            var client = new PirateClient(game.PlayerName, s.Ip, 4939);
            var playingGame = new Game();
            game.GameName = s.ServerName;
            game.MaxPlayers = s.MaxPlayers;
            game.Client = client;
            game.Client.NameRequested += this.OnNameRequest;
            game.PlayingGame = playingGame;
            game.Client.SetGame(playingGame);
            game.Client.InitConnection();
            game.State = GameState.InGame;
        }

        /// <summary>
        /// Draw this join game screen on the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            backGround.Draw(spriteBatch);
            xButton.Draw(spriteBatch);
            back.Draw(spriteBatch);
            refreshButton.Draw(spriteBatch);
            joinGameButton.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Server Name", new Vector2(nameSize.X, nameSize.Y), Color.White);
            spriteBatch.DrawString(font, "Players / Max", new Vector2(playersSize.X, playersSize.Y), Color.White);
            spriteBatch.DrawString(font, "Ip Address", new Vector2(ipSize.X, ipSize.Y), Color.White);
            if(refreshing) {
                spriteBatch.DrawString(
                    font,
                    "Refreshing....",
                    new Vector2(serversRectangle.X, serversRectangle.Y + 30),
                    Color.White);
            } else if(servers.Count > 0) {
                foreach(var s in servers) {
                    s.Draw(spriteBatch);
                }
            } else {
                spriteBatch.DrawString(
                    font,
                    "No servers available right now",
                    new Vector2(serversRectangle.X, serversRectangle.Y + 30),
                    Color.White);
            }
        }

        /// <summary>
        /// Helper method for the event NameRequested
        /// </summary>
        /// <param name="pclient">The client</param>
        private void OnNameRequest(PirateClient pclient) {
            Contract.Ensures(pclient.Name == game.PlayerName);
            var playerName = game.PlayerName;
            pclient.SetName(playerName);
        }
    }
}
