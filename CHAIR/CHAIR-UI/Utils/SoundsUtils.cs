using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CHAIR_UI.Utils
{
    public class SoundsUtils
    {
        private MediaPlayer _onlinePlayer { get; set; }
        private MediaPlayer _offlinePlayer { get; set; }
        private MediaPlayer _messagePlayer { get; set; }

        public SoundsUtils()
        {
            _onlinePlayer = new MediaPlayer();
            _offlinePlayer = new MediaPlayer();
            _messagePlayer = new MediaPlayer();

            _onlinePlayer.Open(new Uri(@"../../Assets/online.mp3", UriKind.Relative));
            _offlinePlayer.Open(new Uri(@"../../Assets/offline.mp3", UriKind.Relative));
            _messagePlayer.Open(new Uri(@"../../Assets/message.mp3", UriKind.Relative));
        }

        public void PlayOnlineSound()
        {
            _onlinePlayer.Stop();
            _onlinePlayer.Play();
        }

        public void PlayOfflineSound()
        {
            _offlinePlayer.Stop();
            _offlinePlayer.Play();
        }

        public void PlayMessageSound()
        {
            _messagePlayer.Stop();
            _messagePlayer.Play();
        }
    }
}
