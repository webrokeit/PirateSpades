// <copyright file="ServerSprite.cs">
//      mche@itu.dk
// </copyright>
// <summary>
//      Class used for representing a server graphically
// </summary>
// <author>Morten Chabert Eskesen (mche@itu.dk)</author>
﻿
namespace PirateSpadesGame.Misc {
    using System.Diagnostics.Contracts;
    using System.Net;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using PirateSpadesGame.GameModes;

    /// <summary>
    /// Class used for representing a server graphically
    /// </summary>
    public class ServerSprite {
        private Rectangle rect;
        private Rectangle ipSize;
        private Rectangle nameSize;
        private Rectangle playersSize;
        private Texture2D tex;
        private int maxPlayers;
        private int players;
        private IPAddress ip;
        private string name;
        private SpriteFont font;
        private int serverNumber;
        private bool mousepressed = false;
        private bool prevmousepressed = false;
        private Rectangle wholeServer;
        private double frametime;
        private double time;

        /// <summary>
        /// The constructor for the class ServerSprite
        /// </summary>
        /// <param name="ip">The server ip</param>
        /// <param name="name">The name of the server</param>
        /// <param name="players">The amount of players currently in the game</param>
        /// <param name="maxPlayers">The maximum amount of players allowed in game</param>
        /// <param name="rect">The rectangle to define the size of the serversprite graphically</param>
        /// <param name="serverNumber">The server's number in the list</param>
        public ServerSprite(IPAddress ip, string name, int players, int maxPlayers, Rectangle rect, int serverNumber) {
            Contract.Requires(ip != null && name != null && players > 0 && maxPlayers > 0 && serverNumber > 0);
            this.ip = ip;
            this.name = name;
            this.players = players;
            this.maxPlayers = maxPlayers;
            this.rect = rect;
            this.serverNumber = serverNumber;
            Selected = false;
            DoubleClick = false;
            Color = Color.White;
            Timer = 0.0;
            State = BState.Up;
            this.SetUp();
        }

        /// <summary>
        /// The state of the serversprite
        /// </summary>
        public BState State { get; private set; }

        /// <summary>
        /// A timer to help determine clicks
        /// </summary>
        public double Timer { get; set; }

        /// <summary>
        /// Set up this server sprite
        /// </summary>
        private void SetUp() {
            var width = rect.Width / 3;
            var height = 30;
            var y = rect.Y + height * serverNumber;
            wholeServer = new Rectangle(rect.X, y, rect.Width, height);
            nameSize = new Rectangle(rect.X, y, width, height);
            var x = rect.X + width;
            playersSize = new Rectangle(x, y, width, height);
            x += width;
            ipSize = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// The ip of the server
        /// </summary>
        public IPAddress Ip { get { return ip; } }

        /// <summary>
        /// The name of the server
        /// </summary>
        public string ServerName { get { Contract.Ensures(ServerName != null); return name; } }

        /// <summary>
        /// The maximum amount of players allowed in the game
        /// </summary>
        public int MaxPlayers { get { Contract.Ensures(MaxPlayers > 0); return maxPlayers; } }

        /// <summary>
        /// Has this serversprite been doubleclicked?
        /// </summary>
        public bool DoubleClick { get; set; }

        /// <summary>
        /// Has this serversprite been selected?
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Change or get the color of this serversprite
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Load the content for this serversprite
        /// </summary>
        /// <param name="contentManager">
        /// The ContentManager used to load the content
        /// </param>
        public void LoadContent(ContentManager contentManager) {
            tex = contentManager.Load<Texture2D>("servertex");
            font = contentManager.Load<SpriteFont>("font");
        }

        /// <summary>
        /// Update this serversprite
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            var state = Mouse.GetState();
            frametime = gameTime.ElapsedGameTime.Milliseconds / 1000.0;
            this.UpdateMovement(state);
        }

        /// <summary>
        /// A helper method for updating the mousemovement on the specific serversprite
        /// </summary>
        /// <param name="state">The current mouse state</param>
        private void UpdateMovement(MouseState state) {
            int mx = state.X;
            int my = state.Y;
            prevmousepressed = mousepressed;
            mousepressed = state.LeftButton == ButtonState.Pressed && PsGame.Active && state.X >= 0 && state.X < PsGame.Width && state.Y >= 0 && state.Y < PsGame.Height;

            if(Func.HitAlpha(wholeServer, tex, mx, my)) {
                Timer = 0.0;
                if(mousepressed) {
                    State = BState.Down;
                } else if(!mousepressed && prevmousepressed && State == BState.Down) {
                    State = BState.JustReleased;
                } else {
                    State = BState.Hover;
                    Color = Color.LightBlue;
                }
            } else {
                State = BState.Up;
                if(Timer > 0) {
                    Timer = Timer - frametime;
                    Color = Color.DarkBlue;
                } else {
                    Color = Color.Transparent;
                }
            }
            if(State == BState.JustReleased) {
                bool wasSelected = Selected;
                lock(JoinGame.Serversprites) {
                    foreach (var s in JoinGame.Serversprites) {
                        s.Selected = false;
                    }
                }
                if(wasSelected) {
                    DoubleClick = true;
                } else {
                    Color = Color.Green;
                    Selected = true;
                }
            }
        }

        /// <summary>
        /// Draw this serversprite on the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(this.tex, this.wholeServer, this.Selected ? Color.Turquoise : this.Color);
            spriteBatch.DrawString(font, name, new Vector2(nameSize.X, nameSize.Y), Color.White);
            spriteBatch.DrawString(font, this.players + "/"+this.maxPlayers, new Vector2(playersSize.X, playersSize.Y), Color.White);
            spriteBatch.DrawString(font, ip.ToString(), new Vector2(ipSize.X, ipSize.Y), Color.White);
        }
    }
}
