using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using PirateSpadesGame;

namespace PirateSpadesGame {
    using PirateSpadesGame.GameModes;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PsGame : Microsoft.Xna.Framework.Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private IGameMode gameMode;
        private Rectangle textbox;
        private Texture2D debugColor;
        private SpriteFont font;
        private String text;
        private String parsedText;
        private String typedText;
        private double typedTextLength;
        private int delayInMilliseconds;
        private bool isDoneDrawing;


        public PsGame() {
            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1024, PreferredBackBufferHeight = 720 };
            Content.RootDirectory = "Content";
            State = GameState.StartUp;
            Color = Color.CornflowerBlue;
        }

        public static GameState State { get; set; }

        public static Color Color { get; set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;

            gameMode = new StartUp(Window);
            textbox = new Rectangle(10, 10, 300, 300);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            gameMode.LoadContent(this.Content);
            debugColor = Content.Load<Texture2D>("solidred");
            font = Content.Load<SpriteFont>("font");
            text = "Hello World";
            parsedText = ParseText(text);
            delayInMilliseconds = 50;
            isDoneDrawing = false;
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

            // TODO: Add your update logic here

            switch(State) {
                case GameState.StartUp:
                    if(!(gameMode is StartUp)) {
                        gameMode = new StartUp(Window);
                    }
                    gameMode.Update(gameTime);
                    break;
                case GameState.InGame:
                    break;
                case GameState.JoinGame:
                    break;
                case GameState.CreateGame:
                    break;
                case GameState.Exit:
                    this.Exit();
                    break;
            }

            if(!isDoneDrawing) {
                if(delayInMilliseconds == 0) {
                    typedText = parsedText;
                    isDoneDrawing = true;
                } else if(typedTextLength < parsedText.Length) {
                    typedTextLength = typedTextLength + gameTime.ElapsedGameTime.TotalMilliseconds / delayInMilliseconds;

                    if(typedTextLength >= parsedText.Length) {
                        typedTextLength = parsedText.Length;
                        isDoneDrawing = true;
                    }

                    typedText = parsedText.Substring(0, (int)typedTextLength);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            gameMode.Draw(this.spriteBatch);
            //spriteBatch.Draw(debugColor, textbox, Color.White);
            spriteBatch.DrawString(font, typedText, new Vector2(textbox.X, textbox.Y), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        private String ParseText(String textToParse) {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = textToParse.Split(' ');

            foreach(String word in wordArray) {
                if(font.MeasureString(line + word).Length() > textbox.Width) {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }

                line = line + word + ' ';
            }

            return returnString + line;
        }
    }

    public enum GameState {
        StartUp, InGame, JoinGame, CreateGame, Exit
    }
}
