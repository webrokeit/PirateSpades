using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpadesGame.Music {
    using System.ComponentModel;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Media;

    class SongPlayer {
        private static SongPlayer me = null;

        public PlayList PlayList { get; private set; }

        public bool Playing { get; private set; }
        private SongPlayer(ContentManager contentManager) {
            PlayList = new PlayList(contentManager);
            PlayList.NextSong += NextSong;

            PlayList.AddMany(
                new []
                    {
                        "audio/pirates_of_the_caribbean", "audio/devils_dance_floor", "audio/hes_a_pirate_tiesto",
                        "audio/requiem_for_a_dying_song", "audio/salty_dog"
                    });

            MediaPlayer.MediaStateChanged += this.MediaStateChange;
        }

        public static SongPlayer GetInstance(ContentManager contentManager) {
            return me ?? (me = new SongPlayer(contentManager));
        }

        public void Start() {
            Playing = true;
            PlayList.Next();
            this.UpdateState();
        }

        public void Stop() {
            Playing = false;
            this.UpdateState();
        }

        private void NextSong(Song song) {
            this.UpdateState();
        }

        private void UpdateState() {
            if(Playing) {
               MediaPlayer.Play(PlayList.SongPlaying);
            }else {
                MediaPlayer.Stop();
            }
        }

        private void MediaStateChange(object sender, EventArgs e) {
            if(MediaPlayer.State == MediaState.Stopped) {
                PlayList.Next();
            }
        }
    }
}
