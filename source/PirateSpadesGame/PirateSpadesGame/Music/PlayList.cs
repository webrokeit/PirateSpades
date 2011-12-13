// <copyright file="PlayList.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A simplistic implementation of a playlist to be used with the XNA MediaPlayer.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpadesGame.Music {
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Media;

    using PirateSpades.Misc;

    /// <summary>
    /// A simplistic implementation of a playlist to be used with the XNA MediaPlayer.
    /// </summary>
    class PlayList {
        /// <summary>
        /// The ContentManager used to load content.
        /// </summary>
        private ContentManager ContentManager { get; set; }

        /// <summary>
        /// Whether or not the repeat the list once the last song has been played.
        /// </summary>
        public bool RepeatList { get; set; }

        /// <summary>
        /// Whether or not to play the list in a shuffled manor.
        /// </summary>
        public bool ShuffleList { get; set; }

        /// <summary>
        /// Whether or not to start playing the first track on the list when the next song is to be played.
        /// </summary>
        private bool RestartAfterSong { get; set; }

        /// <summary>
        /// Index of the currently playing song.
        /// </summary>
        private int CurrentPlaying { get; set; }

        /// <summary>
        /// List of asset names for the songs.
        /// </summary>
        private List<string> SongNames { get; set; }

        /// <summary>
        /// List of ids corresponding to an asset name from the SongNames list.
        /// </summary>
        private List<int> ShuffledList { get; set; }

        /// <summary>
        /// The Song currently being played.
        /// </summary>
        public Song SongPlaying { get; private set; }

        /// <summary>
        /// Delegate used for events.
        /// </summary>
        /// <param name="previous">The song that was just played.</param>
        /// <param name="next">The next song to be played.</param>
        public delegate void SongDelegate(Song previous, Song next);

        /// <summary>
        /// Fires when the Next() method has been called.
        /// </summary>
        public event SongDelegate NextSong;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contentManager">The ContentManager used to load content.</param>
        public PlayList(ContentManager contentManager) {
            this.ContentManager = contentManager;
            CurrentPlaying = -1;
            SongNames = new List<string>();
            this.UpdateShuffleList();
        }

        /// <summary>
        /// Add a song to the list.
        /// </summary>
        /// <param name="songAssetName">The asset name of the song.</param>
        public void Add(string songAssetName) {
            SongNames.Add(songAssetName);
            this.UpdateShuffleList();
        }

        /// <summary>
        /// Add multiple songs to the list.
        /// </summary>
        /// <param name="songAssetNames">A bunch of asset names of songs to add.</param>
        public void AddMany(IEnumerable<string> songAssetNames) {
            SongNames.AddRange(songAssetNames);
            this.UpdateShuffleList();
        }

        /// <summary>
        /// Play next song.
        /// </summary>
        public void Next() {
            if (CurrentPlaying + 1 >= SongNames.Count) {
                CurrentPlaying = -1;
                this.UpdateShuffleList();
                if(!RepeatList) {
                    return;
                }
            }
            if(RestartAfterSong) CurrentPlaying = -1;
            RestartAfterSong = false;

            CurrentPlaying++;
            var songName = SongNames[CurrentPlaying];
            if(ShuffleList) {
                songName = SongNames[ShuffledList[CurrentPlaying]];
            }
            var curSong = SongPlaying;
            SongPlaying = ContentManager.Load<Song>(songName);
            if (NextSong != null) NextSong(curSong, SongPlaying);
        }

        /// <summary>
        /// Update/recreate the shuffled list.
        /// </summary>
        private void UpdateShuffleList() {
            Contract.Ensures(RestartAfterSong = true);
            ShuffledList = new List<int>();
            for(var i = 0; i < SongNames.Count; i++) {
                ShuffledList.Add(i);
            }
            CollectionFnc.FisherYatesShuffle(this.ShuffledList);
            RestartAfterSong = true;
        }
    }
}
