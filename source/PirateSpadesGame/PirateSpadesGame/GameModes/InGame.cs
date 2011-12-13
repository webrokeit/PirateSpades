// <copyright file="InGame.cs">
//      mche@itu.dk, hclk@itu.dk
// </copyright>
// <summary>
//      Class used for making the ingame screen
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>
// <author>Helena Charlotte Lyn Krüger (hclk@itu.dk)</author>

namespace PirateSpadesGame.GameModes {
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using PirateSpades.Network;
    using PirateSpadesGame.IngameFunc;
    using PirateSpadesGame.Misc;
    using Game = PirateSpades.GameLogic.Game;

    /// <summary>
    /// Class used for making the ingame screen
    /// </summary>
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
        private TableSprite playingGround;
        private Rectangle ingameBottom;
        private Texture2D bottom;
        private bool cardRequested;
        private bool finished = false;

        /// <summary>
        /// The constructor for InGame takes a PsGame
        /// </summary>
        /// <param name="game">The currently running PsGame</param>
        public InGame(PsGame game) {
            Contract.Requires(game != null);
            this.game = game;
            this.SetUp();
        }

        /// <summary>
        /// Set up the ingame screen
        /// </summary>
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

            ingameBottom = new Rectangle(0,615, 1024, 120);
        }

        /// <summary>
        /// Load the content of this ingame screen
        /// </summary>
        /// <param name="contentManager">The ContentManager used to load the content</param>
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
            bottom = contentManager.Load<Texture2D>("bottom");
        }

        /// <summary>
        /// Update this ingame screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            if(finished) {
                leaveGame = new Button("leavegame", game.Window.ClientBounds.Width / 2 - Button.Width / 2, game.Window.ClientBounds.Height - Button.Width);
                leaveGame.LoadContent(game.Content);
                if(leaveGame.Update(gameTime)) {
                    this.ButtonAction(leaveGame);
                }
            }
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
                if(playingGround == null) {
                    playingGround = new TableSprite(game, client.Game, new Rectangle(0,0,1024,615));
                    playingGround.LoadContent(game.Content);
                }
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
                    this.showScoreboard = true;
                } else if(currentKeyboardState.IsKeyUp(Keys.Tab)) {
                    this.showScoreboard = false;
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
                            break;
                        }
                    }
                    if(cardToPlay != null && cardRequested && client.CardPlayable(cardToPlay.Card, client.Game.Round.BoardCards.FirstCard)) {
                        this.PlayCard();
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
                playingGround.Update(gameTime);
            }
        }

        /// <summary>
        /// Helper method for playing a card represented by a double clicked CardSprite
        /// </summary>
        private void PlayCard() {
            Contract.Requires(cardToPlay.Card != null && cardToPlay != null && cards.Contains(cardToPlay) && client.HasCard(cardToPlay.Card));
            Contract.Ensures(!client.HasCard(Contract.OldValue(cardToPlay.Card)) && !cards.Contains(cardToPlay) && cardToPlay == null && cardRequested == false);
            client.PlayCard(cardToPlay.Card);
            cards.Remove(cardToPlay);
            cardToPlay = null;
            cardRequested = false;
        }

        /// <summary>
        /// Checks if Escape has been pressed and released
        /// </summary>
        /// <returns>Returns True if Escape has been pressed and released</returns>
        private bool CheckEscape() {
            Contract.Ensures(Contract.Result<bool>() == lastKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyUp(Keys.Escape));
            return lastKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyUp(Keys.Escape);
        }

        /// <summary>
        /// Helper method for taking action upon a button press
        /// </summary>
        /// <param name="b">The button that has been pressed</param>
        private void ButtonAction(Button b) {
            Contract.Requires(b != null);
            var str = b.Name;
            switch(str) {
                case "startgame":
                    if(host.Game.Players.Count >= Game.MinPlayersInGame && host.Game.Players.Count <= Game.MaxPlayersInGame) {
                        host.StartGame();
                        
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

        /// <summary>
        /// Draw this ingame screen on the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            if(this.finished) {
                this.DrawScoreboard(spriteBatch);
                if(leaveGame != null) {
                    leaveGame.Draw(spriteBatch);
                }
                return;
            }
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
                spriteBatch.Draw(bottom, ingameBottom, Color.White);
                if(playingGround != null) {
                    playingGround.Draw(spriteBatch);
                }
                this.DrawRoundScore(spriteBatch);
                if(cards.Count > 0) {
                    foreach(var c in cards) {
                        c.Draw(spriteBatch);
                    }
                }
                if(client.Game.Round.CurrentPlayer == client.Game.PlayerIndex(client)) {
                    spriteBatch.DrawString(font2, "Your Turn!", new Vector2(1024-175, 500), Color.Red);
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
        
        /// <summary>
        /// Draws the scoreboard when the user press' the tab button. Writes the score for each player and round if and only if the 
        /// round is finished or the game is finished. 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <summary>
        /// Draw the scoreboard on the given SpriteBatch
        /// The points for each player
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        private void DrawScoreboard(SpriteBatch spriteBatch) {
            //Create rectangle
            var rectX = game.Window.ClientBounds.Width / 2 - 700 / 2;
            var rectY = game.Window.ClientBounds.Height / 2 - 650 / 2;
            var rect = new Rectangle(rectX, rectY, 700, 650);
            spriteBatch.Draw(board, rect, Color.White);
            
            //How many players should be written on the scoreboard and the width of the name field
            int playerCount = 1+client.Game.Players.Count;
            var rectWidths = rect.Width / playerCount;
            var rectHeight = rect.Height / 22;
            var nameRect = new Rectangle(rect.X, rect.Y, rectWidths, rectHeight);
           
            spriteBatch.DrawString(font2, "Round", new Vector2(nameRect.X + 5, nameRect.Y +5), Color.Black);
            
            var tempX = nameRect.X + rectWidths;
            var tempY = nameRect.Y + rectHeight;
            //get the dictionary from client.game
            var scores = client.Game.GetScoreTable();
            foreach (var play in scores[1].Keys) {
                //write the players name (play.name)
                spriteBatch.DrawString(font2, play.Name, new Vector2(tempX, nameRect.Y + 5), Color.Black );
                tempX += rectWidths;
            }
            
            tempY = nameRect.Y + 5 + rectHeight;
            foreach (var round in scores) {
                if (client.Game.CurrentRound > round.Key || client.Game.Finished) {
                    //We reset the tempX
                    tempX = nameRect.X + 5;
                    //Write the number of the round (round.key)
                    spriteBatch.DrawString(font2, round.Key.ToString(), new Vector2(tempX, tempY), Color.DarkRed);
                    tempX += rectWidths;

                    var roundScore = client.Game.GetRoundScore(round.Key);
                    foreach (var s in round.Value) {
                        //Write s
                        spriteBatch.DrawString(font2, s.Value.ToString() + " (" + roundScore[s.Key] + ")", new Vector2(tempX, tempY), Color.DarkRed);
                        tempX += rectWidths;
                    }
                    tempY += rectHeight;
                }
            }
        }

        /// <summary>
        /// Draw the round score on the given SpriteBatch
        /// The amount of tricks and what the player has bet in the current round
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        private void DrawRoundScore(SpriteBatch spriteBatch) {
            spriteBatch.Draw(scoreOverlay, scoreOverlayRect, Color.White);
            var nameRect = new Rectangle(scoreRectangle.X,  scoreRectangle.Y, scoreRectangle.Width, scoreRectangle.Height);
            var betRect = new Rectangle(scoreRectangle.X, scoreRectangle.Y, 50, scoreRectangle.Height);
            var tricksRect = new Rectangle(scoreRectangle.X, scoreRectangle.Y, 50, scoreRectangle.Height);
            var y = scoreRectangle.Y + scoreRectangle.Height;
            foreach(var p in client.Game.Players) {
                var betted = client.Game.Round.PlayerBets[p];
                var tricks = client.Game.Round.PlayerTricks[p].Count;
                spriteBatch.DrawString(font2, p.Name, new Vector2(nameRect.X, y), p.Name == game.PlayerName ? Color.DarkBlue : Color.DarkRed);
                y += scoreRectangle.Height;
                spriteBatch.DrawString(font2, "Bet: " + (betted == -1 ? "?" : betted.ToString()), new Vector2(betRect.X, y), p.Name == game.PlayerName ? Color.DarkBlue : Color.DarkRed);
                y += scoreRectangle.Height;
                spriteBatch.DrawString(font2, "Tricks: " + tricks, new Vector2(tricksRect.X, y), p.Name == game.PlayerName ? Color.DarkBlue : Color.DarkRed);
                y += 40;
            }
        }

        /// <summary>
        /// Helper method for the event GameFinished
        /// </summary>
        /// <param name="g">The game</param>
        private void OnGameFinished(Game g) {
            Contract.Ensures(g.Finished ? finished && showScoreboard : !finished && !showScoreboard);
            if(g.Finished) {
                showScoreboard = true;
                finished = true;
            }
        }

        /// <summary>
        /// Helper method for the event GameStarted
        /// </summary>
        /// <param name="g">The game</param>
        private void OnGameStarted(Game g) {
            Contract.Ensures(g.Started ? playing : !playing);
            if(g.Started) {
                playing = true;
            }
        }

        /// <summary>
        /// Helper method for the event Disconnected
        /// </summary>
        /// <param name="pc">The client</param>
        private void OnDisconnected(PirateClient pc) {
            Contract.Ensures(!hosting ? game.State == GameState.StartUp : game.State == GameState.InGame);
            if(!hosting) {
                game.State = GameState.StartUp;
            }
        }

        /// <summary>
        /// Helper method for the event BetRequested
        /// </summary>
        /// <param name="pc">The client</param>
        private void OnBetRequest(PirateClient pc) {
            while(!hasBet) { }
            var i = betBox.ParseInput();
            client.SetBet(i);
            betBox.Number = i;
            betBox.Text = betBox.Number.ToString();
            hasBet = false;
        }

        /// <summary>
        /// Helper method for the event CardRequested
        /// </summary>
        /// <param name="pc">The client</param>
        private void OnCardRequest(PirateClient pc) {
            if(cardToPlay != null && cardToPlay.Card != null && client.CardPlayable(cardToPlay.Card)) {
                this.PlayCard();
            } else {
                cardRequested = true;
            }
        }
    }
}
