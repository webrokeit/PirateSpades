// <copyright file="SongPlayer.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      Wrapper for the XNA MediaPlayer.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpadesGame.Music {
    using System;
    using System.Diagnostics.Contracts;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Media;

    /// <summary>
    /// Wrapper for the XNA MediaPlayer.
    /// </summary>
    class SongPlayer {
        /// <summary>
        /// Variable used for lazy initialization of the SongPlayer.
        /// </summary>
        private static SongPlayer me = null;

        /// <summary>
        /// Is a song being playing or not.
        /// </summary>
        public bool IsPlaying {
            get {
                return MediaPlayer.State == MediaState.Playing;
            }
        }

        /// <summary>
        /// The playlist being used for the SongPlayer.
        /// </summary>
        public PlayList PlayList { get; private set; }

        /// <summary>
        /// Value indicating whether or not the SongPlayer has been started.
        /// </summary>
        private bool Playing { get; set; }

        /// <summary>
        /// Constructor, used privately from the Factory method GetInstance().
        /// </summary>
        /// <param name="contentManager">The ContentManager used to load the content.</param>
        private SongPlayer(ContentManager contentManager) {
            PlayList = new PlayList(contentManager);
            PlayList.NextSong += NextSong;

            PlayList.AddMany(
                new []
                    {
                        "audio/pirates_of_the_caribbean", "audio/devils_dance_floor", "audio/hes_a_pirate_tiesto",
                        "audio/requiem_for_a_dying_song", "audio/salty_dog", "audio/drunken_lullabies"
                    });

            MediaPlayer.MediaStateChanged += this.MediaStateChange;
        }

        /// <summary>
        /// Get an instance of the SongPlayer.
        /// </summary>
        /// <param name="contentManager">The ContentManager used to load the content.</param>
        /// <returns>The lazy initialized instance of the SongPlayer.</returns>
        public static SongPlayer GetInstance(ContentManager contentManager) {
            return me ?? (me = new SongPlayer(contentManager));
        }

        /// <summary>
        /// Start the SongPlayer.
        /// </summary>
        public void Start() {
            Contract.Ensures(Playing = true);
            Playing = true;
            PlayList.Next();
            this.UpdateState();
        }

        /// <summary>
        /// Stop the SongPlayer.
        /// </summary>
        public void Stop() {
            Contract.Ensures(Playing = false);
            Playing = false;
            this.UpdateState();
        }

        /// <summary>
        /// Event used as a handler for PlayList.NextSong.
        /// </summary>
        /// <param name="previous">The song that was previously playing.</param>
        /// <param name="next">The song to be played.</param>
        private void NextSong(Song previous, Song next) {
            MediaPlayer.Stop();
            if(previous != null) previous.Dispose();
            this.UpdateState();
        }

        /// <summary>
        /// Plays next song or stops the MediaPlayer if we do no longer want something playing
        /// </summary>
        private void UpdateState() {
            if(Playing) {
               MediaPlayer.Play(PlayList.SongPlaying);
            }else {
                MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Handler for the MediaLibrary.MediaStateChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void MediaStateChange(object sender, EventArgs e) {
            if(MediaPlayer.State == MediaState.Stopped) {
                PlayList.Next();
            }
        }
    }
}
