//Helena
namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using PirateSpades.Misc;
    using PirateSpades.GameLogic;
    using PirateSpades.Network;

    using Game = PirateSpades.GameLogic.Game;

    public class InGame : IGameMode {
        private Vector2 screenCenter;
        private Dictionary<string, Dictionary<string, int>> score;
        private bool playing = false;
        private bool showMenu = false;
        private bool showScoreboard = false;
        private PirateClient client;
        private PirateHost host;
        private Game playingGame;
        private bool hosting;
        private Sprite menu;
        private Sprite inJoinbackGround;
        private Button leaveGame;
        private Button startGame;
        private Button exitGame;
        private Button resumeGame;
        private Button menuleaveGame;
        private List<Button> menuButtons; 
        private PsGame game;
        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;
        private int maxPlayers;
        private int players;
        private string gameName;
        private Rectangle namesRectangle;
        private SpriteFont font;
        private Vector2 gamePos;
        private Vector2 playerPos;
        private Vector2 maxPlayerPos;
        private List<CardSprite> cards;
        public static IList<CardSprite> Cardsprites = new List<CardSprite>().AsReadOnly();
        private CardSprite cardToPlay;
        private Button bet;
        private Rectangle cardSize;
        private Numberbox betBox;
        private bool hasBet = false;
        private Texture2D cardback;

        public InGame(PsGame game) {
            this.game = game;
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
            gameName = game.GameName;
            maxPlayers = game.MaxPlayers;

            client.Disconnected += OnDisconnected;
            client.BetRequested += OnBetRequest;
            client.CardRequested += OnCardRequest;
            client.Game.RoundStarted += OnGameStarted;

            cards = new List<CardSprite>();

            inJoinbackGround = new Sprite() {Color = Color.White};
            var x = game.Window.ClientBounds.Width / 2 - 400 / 2;
            var y = game.Window.ClientBounds.Height / 2 - 500 / 2;
            inJoinbackGround.Position = new Vector2(x,y);

            leaveGame = new Button("leavegame", x+250, y+450);

            if(hosting) {
                startGame = new Button("startgame", x, y + 450);
            }

            playerPos = new Vector2(x+20,y+85);

            maxPlayerPos = new Vector2(x+175, y+85);

            gamePos = new Vector2(x + 5, y + 60);

            namesRectangle = new Rectangle(x+5, y+150, 200, 30);

            menu = new Sprite() { Color = Color.White };
            var menuX = game.Window.ClientBounds.Width / 2 - 300 / 2;
            var menuY = game.Window.ClientBounds.Height / 2 - 420 / 2;
            menu.Position = new Vector2(menuX, menuY);
            menuX += 75;
            menuY += 100;
            resumeGame = new Button("resumegame", menuX, menuY);
            menuY += Button.Height;
            menuleaveGame = new Button("leavegame", menuX, menuY);
            menuY += Button.Height;
            exitGame = new Button("exitgame", menuX, menuY);
            menuButtons = new List<Button>() { this.menuleaveGame, this.resumeGame, this.exitGame };

            var rect = new Rectangle(x + 250, y + 200, 100, 50);
            betBox = new Numberbox(rect, "volumebox", 2) { Limit = 10, Number = 0 };
            betBox.Text = betBox.Number.ToString();

            var betX = 925;
            var betY = 675;
            bet = new Button("bet", betX, betY);

            cardSize = new Rectangle(5, 650, 75, 120);
        }

        public void LoadContent(ContentManager contentManager) {
            if(hosting) {
                startGame.LoadContent(contentManager);
            }
            inJoinbackGround.LoadContent(contentManager, "ingamewindow");
            menu.LoadContent(contentManager, "menubackground");
            exitGame.LoadContent(contentManager);
            resumeGame.LoadContent(contentManager);
            menuleaveGame.LoadContent(contentManager);
            leaveGame.LoadContent(contentManager);
            font = contentManager.Load<SpriteFont>("font");
            bet.LoadContent(contentManager);
            cardback = contentManager.Load<Texture2D>("cardback");
        }

        public void Update(GameTime gameTime) {
            if(!playing) {
                if(hosting) {
                    if(startGame.Update(gameTime)) {
                        this.ButtonAction(startGame);
                    }
                }

                players = client.Game.Players.Count;
                if(leaveGame.Update(gameTime)) {
                    this.ButtonAction(leaveGame);
                }

            } else {
                currentKeyboardState = Keyboard.GetState();
                if(this.CheckEscape()) {
                    this.showMenu = !this.showMenu;
                }
                lastKeyboardState = currentKeyboardState;
                if(showMenu) {
                    foreach(var b in this.menuButtons.Where(b => b.Update(gameTime))) {
                        this.ButtonAction(b);
                    }
                }
                if(currentKeyboardState.IsKeyDown(Keys.Tab)) {
                    this.showScoreboard = !this.showScoreboard;
                }
                if(bet.Update(gameTime)) {
                    this.ButtonAction(bet);
                }
                if(cards.Count > 0) {
                    foreach (var c in this.cards.Where(c => c.DoubleClick)) {
                        this.cardToPlay = c;
                    }
                } else if(client.Hand.Count > 0 && cards.Count == 0) {
                    betBox.Limit = client.Hand.Count;
                    var tempX = cardSize.X;
                    foreach(var c in client.Hand) {
                        var cs = new CardSprite(c, new Rectangle(tempX, cardSize.Y, cardSize.Width, cardSize.Width));
                        cs.LoadContent(game.Content);
                        cards.Add(cs);
                        tempX += cardSize.Width;
                    }
                }
            }
        }

        private bool CheckEscape() {
            return lastKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyUp(Keys.Escape);
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
                case "bet":
                    if(client.Game.Round.PlayerBets.ContainsKey(client)) {
                        return;
                    }
                    hasBet = true;
                    break;
                case "resumegame":
                    showMenu = false;
                    break;
                case "leavegame":

                    game.Client = null;
                    if(hosting) {
                        host.Stop();
                        game.Host = null;
                    }
                    game.State = GameState.StartUp;
                    break;
                case "exitgame":

                    game.Client = null;
                    if(hosting) {
                        host.Stop();
                        game.Host = null;
                    }
                    game.State = GameState.Exit;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            if(!playing) {
                inJoinbackGround.Draw(spriteBatch);
                leaveGame.Draw(spriteBatch);
                if(hosting) {
                    startGame.Draw(spriteBatch);
                }
                spriteBatch.DrawString(font, "Server Name: "+ gameName, gamePos, Color.White);
                spriteBatch.DrawString(font, "Players: "+players, playerPos, Color.White);
                spriteBatch.DrawString(font, "Max Players: "+maxPlayers, maxPlayerPos, Color.White);
                spriteBatch.DrawString(
                    font, "The Players in "+gameName+":", new Vector2(namesRectangle.X, namesRectangle.Y), Color.White);
                var y = namesRectangle.Y+namesRectangle.Height;
                foreach(var p in client.Game.Players) {
                    spriteBatch.DrawString(font, p.Name, new Vector2(namesRectangle.X, y), Color.White);
                    y += namesRectangle.Height;
                }
            } else {
                if(cards.Count > 0) {
                    foreach (var c in cards) {
                        c.Draw(spriteBatch);
                    }
                }
                if(showScoreboard) {
                    spriteBatch.DrawString(font, "HEJ SCOREBOARD", new Vector2(game.Window.ClientBounds.Width - 500, game.Window.ClientBounds.Height - 300), Color.White);
                }
                bet.Draw(spriteBatch);
                if (showMenu) {
                    menu.Draw(spriteBatch);
                    exitGame.Draw(spriteBatch);
                    menuleaveGame.Draw(spriteBatch);
                    resumeGame.Draw(spriteBatch);
                }
            }
        }

        private void OnGameStarted(Game g) {
            if(g.Started) {
                playing = true;
            }
        }
 
        private void OnDisconnected(PirateClient pc) {
            if(!hosting) {
                game.State = GameState.StartUp;
            }
        }

        private void OnBetRequest(PirateClient pc) {
            if(hasBet) {
                client.SetBet(betBox.ParseInput());
                hasBet = false;
            }
        }

        private void OnCardRequest(PirateClient pc) {
            if(client.CardPlayable(cardToPlay.Card)) {
                client.PlayCard(cardToPlay.Card);
                cards.Remove(cardToPlay);
                cardToPlay = null;
            }
        }
    }
}
