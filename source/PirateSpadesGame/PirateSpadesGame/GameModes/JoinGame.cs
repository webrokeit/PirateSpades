
namespace PirateSpadesGame.GameModes {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using PirateSpades.GameLogic;
    using PirateSpades.Network;

    using Game = PirateSpades.GameLogic.Game;

    public class JoinGame : IGameMode {
        private PsGame game;
        private bool joinedGame = false;
        private JoinedGame inJoinedGame;
        private Texture2D testTex;
        private Button xButton;
        private Button refreshButton;
        private Button joinGameButton;
        private Button back;
        private Sprite backGround;
        private double frametime;
        private bool mpressed = false;
        private bool prevmpressed = false;
        private Rectangle nameSize;
        private Rectangle ipSize;
        private Rectangle playersSize;
        private SpriteFont font;
        private Rectangle serversRectangle;
        private List<Rectangle> rects;
        private List<ServerSprite> servers;
        private PirateScanner scanner;
        private ContentManager content;
        private bool refreshed = false;
        public static IList<ServerSprite> Serversprites = new List<ServerSprite>().AsReadOnly();
        private List<Button> buttons;
        private int numberOfServers;
        private bool refreshing = false;
        private delegate IList<PirateScanner.GameInfo> ScanForGamesDelegate(int port, int timeout);

        public JoinGame(PsGame game) {
            servers = new List<ServerSprite>();
            this.game = game;
            this.SetUp(game.Window);
            content = game.Content;
            scanner = new PirateScanner();
        }

        private void SetUp(GameWindow window) {
            backGround = new Sprite() { Color = Color.White };
            var x = window.ClientBounds.Width / 2 - 800 / 2;
            var y = window.ClientBounds.Height / 2 - 500 / 2;
            backGround.Position = new Vector2(x, y);

            var serversX = x + 75;
            var serversY = y + 75;

            serversRectangle = new Rectangle(serversX, serversY, 650, 30);
            this.SetUpServers();


            xButton = new Button("X", x + 650, window.ClientBounds.Height / 2 - 500 / 2 - 10);
            var buttonx = x + 650;
            var buttony = window.ClientBounds.Height / 2 + 200;
            back = new Button("backjoingame", buttonx, buttony);
            buttonx -= Button.Width;
            refreshButton = new Button("refresh", buttonx, buttony);
            buttonx -= Button.Width;
            joinGameButton = new Button("jointhisgame", buttonx, buttony);
            buttons = new List<Button> { this.xButton, this.refreshButton, this.joinGameButton, this.back };
        }

        private void SetUpServers() {
            var width = serversRectangle.Width / 3;
            var height = 30;
            nameSize = new Rectangle(serversRectangle.X, serversRectangle.Y, width, height);
            var x = serversRectangle.X + width;
            playersSize = new Rectangle(x, serversRectangle.Y, width, height);
            x += width;
            ipSize = new Rectangle(x, serversRectangle.Y, width, height);
        }

        public void LoadContent(ContentManager contentManager) {
            backGround.LoadContent(contentManager, "findgame");
            xButton.LoadContent(contentManager);
            back.LoadContent(contentManager);
            refreshButton.LoadContent(contentManager);
            joinGameButton.LoadContent(contentManager);
            font = contentManager.Load<SpriteFont>("font");
        }

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

        private void ButtonAction(Button b) {
            if(b == null) {
                return;
            }
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

        private void Refresh() {
            if(refreshing) return;
            refreshing = true;
            numberOfServers = 0;

            const int Port = 4939;
            const int Timeout = 15000;

            scanner.GameFound += GameFound;
            var del = new ScanForGamesDelegate(scanner.ScanForGames);
            del.BeginInvoke(Port, Timeout, RefreshDone, del);
        }

        private void RefreshDone(IAsyncResult ar) {
            var del = (ScanForGamesDelegate)ar.AsyncState;
            var res = del.EndInvoke(ar);

            scanner.GameFound += GameFound;

            // Dette burde være unødvendigt, så det skal lige testes om det skal med eller ej
            lock(servers) {
                //servers.Clear();
                //servers.AddRange(res.Select(gameInfo => new ServerSprite(gameInfo.Ip, gameInfo.GameName, gameInfo.Players, gameInfo.MaxPlayers, serversRectangle)));
            }

            refreshing = false;
            refreshed = true;
        }

        private void GameFound(PirateScanner.GameInfo gameInfo) {
            lock(servers) {
                numberOfServers++;
                var serverSprite = new ServerSprite(gameInfo.Ip, gameInfo.GameName, gameInfo.Players, gameInfo.MaxPlayers, serversRectangle, numberOfServers);
                serverSprite.LoadContent(content);
                servers.Add(serverSprite);
                Serversprites = servers.AsReadOnly();
            }
        }

        private void JoinSelectedGame() {
            foreach(var s in servers.Where(s => s.Selected)) {
                this.JoinThisGame(s);
                return;
            }
        }

        private void JoinThisGame(ServerSprite s) {
            var client = new PirateClient(game.PlayerName, s.Ip, 4939);
            var playingGame = new Game();
            game.GameName = s.ServerName;
            game.MaxPlayers = s.MaxPlayers;
            game.Client = client;
            game.Client.NameRequested += this.OnNameRequest;
            game.PlayingGame = playingGame;
            game.Client.SetGame(playingGame);
            game.Client.Game.RoundStarted += this.OnRoundStart;
            game.Client.InitConnection();
            game.State = GameState.InGame;
        }

        public void Draw(SpriteBatch spriteBatch) {
            backGround.Draw(spriteBatch);
            xButton.Draw(spriteBatch);
            back.Draw(spriteBatch);
            refreshButton.Draw(spriteBatch);
            joinGameButton.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Server Name", new Vector2(nameSize.X, nameSize.Y), Color.White);
            spriteBatch.DrawString(font, "Players / Max", new Vector2(playersSize.X, playersSize.Y), Color.White);
            spriteBatch.DrawString(font, "Ip Address", new Vector2(ipSize.X, ipSize.Y), Color.White);
            if(!refreshed) {
                spriteBatch.DrawString(
                    font,
                    "Press refresh to find servers",
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

        private void OnNameRequest(PirateClient pclient) {
            var playerName = game.PlayerName;
            pclient.SetName(playerName);
        }

        private void OnRoundStart(Game g) {
            
        }
    }
}
