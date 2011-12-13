using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpadesGame.Music {
    using System.ComponentModel;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Media;

    using PirateSpades.Misc;

    class PlayList {
        private ContentManager ContentManager { get; set; }

        public bool RepeatList { get; set; }
        public bool ShuffleList { get; set; }

        private bool RestartAfterSong { get; set; }

        private int CurrentPlaying { get; set; }

        private List<string> SongNames { get; set; }

        private List<int> ShuffledList { get; set; }

        public Song SongPlaying { get; private set; }

        public delegate void SongDelegate(Song song);

        public event SongDelegate NextSong;
        
        public PlayList(ContentManager contentManager) {
            this.ContentManager = contentManager;
            CurrentPlaying = -1;
            SongNames = new List<string>();
            this.UpdateShuffleList();
        }

        public void Add(string songAssetName) {
            SongNames.Add(songAssetName);
            this.UpdateShuffleList();
        }

        public void AddMany(IEnumerable<string> songAssetNames) {
            SongNames.AddRange(songAssetNames);
            this.UpdateShuffleList();
        }

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
            SongPlaying = ContentManager.Load<Song>(songName);
            if (NextSong != null) NextSong(SongPlaying);
        }

        private void UpdateShuffleList() {
            ShuffledList = new List<int>();
            for(var i = 0; i < SongNames.Count; i++) {
                ShuffledList.Add(i);
            }
            CollectionFnc.FisherYatesShuffle(this.ShuffledList);
            RestartAfterSong = true;
        }
    }
}
