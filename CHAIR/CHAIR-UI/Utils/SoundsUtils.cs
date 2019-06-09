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
        private MediaPlayer _playingPlayer { get; set; }

        public SoundsUtils()
        {
            _onlinePlayer = new MediaPlayer();
            _offlinePlayer = new MediaPlayer();
            _messagePlayer = new MediaPlayer();
            _playingPlayer = new MediaPlayer();

            _onlinePlayer.Open(new Uri(@"Assets/online.mp3", UriKind.Relative));
            _offlinePlayer.Open(new Uri(@"Assets/offline.mp3", UriKind.Relative));
            _messagePlayer.Open(new Uri(@"Assets/message.mp3", UriKind.Relative));
            _playingPlayer.Open(new Uri(@"Assets/playing.mp3", UriKind.Relative));

            //Notification sounds are a bit too loud (default is 0.5, so lower than that)
            _offlinePlayer.Volume = 0.3;
            _onlinePlayer.Volume = 0.35;
            _messagePlayer.Volume = 0.4;
        }

        public void PlayOnlineSound()
        {
            if(SettingUtils.getOnlineNotificationSetting())
            {
                _onlinePlayer.Stop();
                _onlinePlayer.Play();
            }
        }

        public void PlayOfflineSound()
        {
            if(SettingUtils.getOfflineNotificationSetting())
            {
                _offlinePlayer.Stop();
                _offlinePlayer.Play();
            }
        }

        public void PlayMessageSound()
        {
            if (SettingUtils.getMessageNotificationSetting())
            {
                _messagePlayer.Stop();
                _messagePlayer.Play();
            }
        }

        public void PlayPlayingSound()
        {
            if (SettingUtils.getPlayingGameNotificationSetting())
            {
                _playingPlayer.Stop();
                _playingPlayer.Play();
            }
        }
    }
}
