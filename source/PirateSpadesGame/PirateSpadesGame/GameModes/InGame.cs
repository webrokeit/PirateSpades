//Helena
namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using PirateSpades.Misc;
    using PirateSpades.Network;
    using Game = PirateSpades.GameLogic.Game;

    public class InGame : IGameMode {
        private bool playing = false;
        private bool showMenu = false;
        private bool showScoreboard = false;
        private PirateClient client;
        private PirateHost host;
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
        private Rectangle scoreRectangle;
        private SpriteFont font2;
        private Texture2D scoreOverlay;
        private Rectangle scoreOverlayRect;
        private Texture2D board;
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
            gameName = game.GameName;
            maxPlayers = game.MaxPlayers;

            client.Disconnected += OnDisconnected;
            client.BetRequested += OnBetRequest;
            client.CardRequested += OnCardRequest;
            client.Game.RoundStarted += OnGameStarted;
            client.Game.GameFinished += OnGameFinished;

            cards = new List<CardSprite>();

            inJoinbackGround = new Sprite { Color = Color.White };
            var x = game.Window.ClientBounds.Width / 2 - 400 / 2;
            var y = game.Window.ClientBounds.Height / 2 - 500 / 2;
            inJoinbackGround.Position = new Vector2(x, y);

            leaveGame = new Button("leavegame", x + 250, y + 450);

            if(hosting) {
                startGame = new Button("startgame", x, y + 450);
            }

            playerPos = new Vector2(x + 20, y + 85);

            maxPlayerPos = new Vector2(x + 175, y + 85);

            gamePos = new Vector2(x + 5, y + 60);

            namesRectangle = new Rectangle(x + 5, y + 150, 200, 30);

            menu = new Sprite { Color = Color.White };
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
            menuButtons = new List<Button> { this.menuleaveGame, this.resumeGame, this.exitGame };
          

            var rect = new Rectangle(900, 520, 100, 50);
            betBox = new Numberbox(rect, "bettingbox", 2) { Limit = 10, Number = 0 };
            betBox.Text = betBox.Number.ToString();

            bet = new Button("bet", 860,  565);

            cardSize = new Rectangle(5, 615, 100, 120);

            scoreRectangle = new Rectangle(1024-175, 0, 75, 20);

            scoreOverlayRect = new Rectangle(1024-177, 0, 177, 520);

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
            font2 = contentManager.Load<SpriteFont>("font2");
            board = contentManager.Load<Texture2D>("Scoreboard");
            bet.LoadContent(contentManager);
            contentManager.Load<Texture2D>("cardback");
            betBox.LoadContent(contentManager);
            scoreOverlay = contentManager.Load<Texture2D>("scoreoverlay");
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
                betBox.Update(gameTime);
                if(cards.Count > 0) {
                    foreach(var c in this.cards) {
                        c.Update(gameTime);
                        if(c.DoubleClick) {
                            this.cardToPlay = c;
                        }
                    }
                } else if(client.Hand.Count == client.Game.CardsToDeal && cards.Count == 0) {
                    betBox.Limit = client.Hand.Count;
                    betBox.Number = 0;
                    betBox.Text = betBox.Number.ToString();
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
                    if(client.Game.Round.PlayerBets[client] > -1) {
                        return;
                    }
                    hasBet = true;
                    break;
                case "resumegame":
                    showMenu = false;
                    break;
                case "leavegame":
                    client.Disconnect();
                    game.Client = null;
                    if(hosting) {
                        host.Stop();
                        game.Host = null;
                    }
                    game.State = GameState.StartUp;
                    break;
                case "exitgame":
                    client.Disconnect();
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
                spriteBatch.DrawString(font, "Server Name: " + gameName, gamePos, Color.White);
                spriteBatch.DrawString(font, "Players: " + players, playerPos, Color.White);
                spriteBatch.DrawString(font, "Max Players: " + maxPlayers, maxPlayerPos, Color.White);
                spriteBatch.DrawString(
                    font, "The Players in " + gameName + ":", new Vector2(namesRectangle.X, namesRectangle.Y), Color.White);
                var y = namesRectangle.Y + namesRectangle.Height;
                foreach(var p in client.Game.Players) {
                    spriteBatch.DrawString(font, p.Name, new Vector2(namesRectangle.X, y), Color.White);
                    y += namesRectangle.Height;
                }
            } else {
                this.DrawRoundScore(spriteBatch);
                if(cards.Count > 0) {
                    foreach(var c in cards) {
                        c.Draw(spriteBatch);
                    }
                }
                betBox.Draw(spriteBatch);
                bet.Draw(spriteBatch);
                if(showScoreboard) {
                    this.DrawScoreboard(spriteBatch);
                }
                if(showMenu) {
                    menu.Draw(spriteBatch);
                    exitGame.Draw(spriteBatch);
                    menuleaveGame.Draw(spriteBatch);
                    resumeGame.Draw(spriteBatch);
                }
            }
        }
        
        private void DrawScoreboard(SpriteBatch spriteBatch) {
            var rectX = game.Window.ClientBounds.Width / 2 - 500 / 2;
            var rectY = game.Window.ClientBounds.Height / 2 - 500 / 2;
            var rect = new Rectangle(rectX, rectY, 700, 650);
            spriteBatch.Draw(board, rect, Color.White);
            
            int playerCount = 1+client.Game.Players.Count;
            var rectWidths = rect.Width / playerCount;
            var rectHeight = rect.Height / 22;
            var nameRect = new Rectangle(rect.X, rect.Y, rectWidths, rectHeight);
            var scores = client.Game.GetScoreTable();
            spriteBatch.DrawString(font2, "Round", new Vector2(nameRect.X + 5, nameRect.Y +5), Color.Black);
            var tempX = nameRect.X + rectWidths;
            var tempY = nameRect.Y + rectHeight;
            foreach (var play in scores[1].Keys) {
                //write play.name
                spriteBatch.DrawString(font2, play.Name, new Vector2(tempX, nameRect.Y + 5), Color.Black );
                tempX += rectWidths;
            }

            tempY = nameRect.Y + 5 + rectHeight;

            foreach (var round in scores) {
                tempX = nameRect.X + 5;
                //Write round.key/int
                spriteBatch.DrawString(font2, round.Key.ToString(), new Vector2(tempX, tempY), Color.Black );
                tempX += rectWidths;
                foreach (var s in round.Value) {
                    //Write s
                    spriteBatch.DrawString(font2, s.Value.ToString(), new Vector2(tempX, tempY), Color.BurlyWood );
                    tempX += rectWidths;
                }
                tempY += rectHeight;
            }
            
            //Total points of a player
            spriteBatch.DrawString(font2, "Total:", new Vector2(nameRect.X + 5, nameRect.Y +5 + (21*rectHeight)), Color.Black);
            tempX = nameRect.X + 5;
            foreach(var p in scores[1].Keys) {
                spriteBatch.DrawString(font2, client.Game.Get , new Vector2(tempX, nameRect.Y +5 + (21*rectHeight)), Color.Black);
                tempX += rectWidths;
            }
    
        }
            
        }

        private void DrawRoundScore(SpriteBatch spriteBatch) {
            Contract.Requires(client.Game.Players.Count > 1);
            spriteBatch.Draw(scoreOverlay, scoreOverlayRect, Color.White);
            var nameRect = new Rectangle(scoreRectangle.X,  scoreRectangle.Y, scoreRectangle.Width, scoreRectangle.Height);
            var betRect = new Rectangle(scoreRectangle.X, scoreRectangle.Y, 50, scoreRectangle.Height);
            var tricksRect = new Rectangle(scoreRectangle.X, scoreRectangle.Y, 50, scoreRectangle.Height);
            var y = scoreRectangle.Y + scoreRectangle.Height;
            foreach(var p in client.Game.Players) {
                var betted = client.Game.Round.PlayerBets[p];
                var tricks = client.Game.Round.PlayerTricks[p].Count;
                spriteBatch.DrawString(font2, p.Name, new Vector2(nameRect.X, y), p.Name == game.PlayerName ? Color.Blue : Color.Black);
                y += scoreRectangle.Height;
                spriteBatch.DrawString(font2, "Bet: " + (betted == -1 ? "?" : betted.ToString()), new Vector2(betRect.X, y), p.Name == game.PlayerName ? Color.Blue : Color.Black);
                y += scoreRectangle.Height;
                spriteBatch.DrawString(font2, "Tricks: " + tricks, new Vector2(tricksRect.X, y), p.Name == game.PlayerName ? Color.Blue : Color.Black);
                y += 40;
            }
        }

        private void OnGameFinished(Game g) {
            if(g.Finished) {
                showScoreboard = true;
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
            while(!hasBet) { }
            var i = betBox.ParseInput();
            client.SetBet(i);
            betBox.Number = i;
            betBox.Text = betBox.Number.ToString();
            hasBet = false;
        }

        private void OnCardRequest(PirateClient pc) {
            while(cardToPlay == null || !client.CardPlayable(cardToPlay.Card)) { }
            if(client.CardPlayable(cardToPlay.Card)) {
                client.PlayCard(cardToPlay.Card);
                cards.Remove(cardToPlay);
                cardToPlay = null;
            }
        }
    }
}
