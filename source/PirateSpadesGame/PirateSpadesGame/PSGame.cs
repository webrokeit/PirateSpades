//Helena
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PirateSpadesGame {
    using System.Text.RegularExpressions;

    using Microsoft.Xna.Framework.Audio;

    using PirateSpades.GameLogic;
    using PirateSpades.Network;


    using PirateSpadesGame.GameModes;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PsGame : Microsoft.Xna.Framework.Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private IGameMode gameMode;
        private StartUp startUp;
        private JoinGame joinGame;
        private CreateGame createGame;
        private InGame inGame;
        private Sprite title;
        private Sprite background;
        private Sprite namePopUp;
        private Button ok;
        private Textbox textbox;
        private bool settingname = false;

        public PsGame() {
            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1024, PreferredBackBufferHeight = 720 };
            Content.RootDirectory = "Content";
            State = GameState.StartUp;
            Color = Color.CornflowerBlue;
            MusicVolume = 1.0f;
            PlayerName = "";
        }

        public PirateHost Host { get; set; }

        public PirateClient Client { get; set; }

        public Game PlayingGame { get; set; }

        public string GameName { get; set; }

        public int MaxPlayers { get; set; }

        public string PlayerName { get; set; }

        public GameState State { get; set; }

        public Color Color { get; set; }

        public float MusicVolume { get; set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            this.IsMouseVisible = true;

            title = new Sprite();
            background = new Sprite();
            var x = this.Window.ClientBounds.Width / 2 - 200;
            title.Position = new Vector2(x, 0);
            startUp = new StartUp(this);
            gameMode = startUp;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background.LoadContent(this.Content, "PIRATESHIP");
            title.LoadContent(this.Content, "pspades");
            gameMode.LoadContent(this.Content);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if(settingname && (State == GameState.JoinGame || State == GameState.CreateGame)) {
                if(ok.Update(gameTime)) {
                    this.ButtonAction(ok);
                }
                textbox.Update(gameTime);
            } else {
                this.GameMode(gameTime);
            }

            base.Update(gameTime);
        }

        private void GameMode(GameTime gameTime) {
            switch(State) {
                case GameState.StartUp:
                    if(!(gameMode is StartUp)) {
                        gameMode = startUp;
                        gameMode.LoadContent(this.Content);
                    }
                    gameMode.Update(gameTime);
                    break;
                case GameState.InGame:
                    if(!(gameMode is InGame)) {
                        inGame = new InGame(this);
                        gameMode = inGame;
                        gameMode.LoadContent(this.Content);
                    }
                    gameMode.Update(gameTime);
                    break;
                case GameState.JoinGame:
                    if(PlayerName == "" || !Regex.IsMatch(PlayerName, @"^\w{3,20}$")) {
                        this.SetName();
                        break;
                    }
                    if(!(gameMode is JoinGame)) {
                        joinGame = new JoinGame(this);
                        gameMode = joinGame;
                        gameMode.LoadContent(this.Content);
                    }
                    gameMode.Update(gameTime);
                    break;
                case GameState.CreateGame:
                    if(PlayerName == "" || !Regex.IsMatch(PlayerName, @"^\w{3,20}$")) {
                        this.SetName();
                        break;
                    }
                    if(!(gameMode is CreateGame)) {
                        createGame = new CreateGame(this);
                        gameMode = createGame;
                        gameMode.LoadContent(this.Content);
                    }
                    gameMode.Update(gameTime);
                    break;
                case GameState.Exit:
                    this.Exit();
                    break;
            }
        }

        private void ButtonAction(Button b) {
            if(b == null) {
                return;
            }
            var str = b.Name;
            switch(str) {
                case "ok":
                    if(Regex.IsMatch(textbox.Text, @"^\w{3,20}$")) {
                        settingname = false;
                        this.PlayerName = textbox.Text;
                    } else {
                        settingname = true;
                    }
                    break;
            }
        }

        private void SetName() {
            settingname = true;
            namePopUp = new Sprite();
            namePopUp.LoadContent(this.Content, "PopUp");
            int x = this.Window.ClientBounds.Width / 2 - namePopUp.Tex.Width / 2;
            int y = this.Window.ClientBounds.Height / 2 - namePopUp.Tex.Height / 2;
            namePopUp.Position = new Vector2(x, y);
            var rect = new Rectangle(x + (namePopUp.Tex.Width - 250), y + 50, 250, 75);
            textbox = new Textbox(rect, "playername") { Text = this.PlayerName, Typable = true };
            textbox.MoveText(45);
            textbox.LoadContent(this.Content);
            int okX = x + namePopUp.Tex.Width / 2 - 75;
            int okY = y + 150;
            ok = new Button("ok", okX, okY);
            ok.LoadContent(this.Content);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color);

            spriteBatch.Begin();
            background.Draw(spriteBatch);
            title.Draw(spriteBatch);
            gameMode.Draw(this.spriteBatch);
            if(settingname) {
                namePopUp.Draw(this.spriteBatch);
                textbox.Draw(this.spriteBatch);
                ok.Draw(this.spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public enum GameState {
        StartUp, InGame, JoinGame, CreateGame, Exit
    }
}
