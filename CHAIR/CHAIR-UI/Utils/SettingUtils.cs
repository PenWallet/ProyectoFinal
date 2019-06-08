using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.Utils
{
    public static class SettingUtils
    {
        public static List<string> scanInstallingFolder()
        {
            List<string> installedGames = new List<string>();

            //This creates the directory if it doesn't exist, and if it does, does nothing
            DirectoryInfo dirInfo = Directory.CreateDirectory(getInstallingFolder());

            string[] subdirectories = Directory.GetDirectories(getInstallingFolder());

            foreach(string gameFolder in subdirectories)
                installedGames.Add(gameFolder.Split('\\').Last());

            return installedGames;
        }

        public static RegistryKey getRegistryKey()
        {
            //No need to check if it exists or not, since this method creates a new one or opens the existing one if already exists
            return Registry.CurrentUser.CreateSubKey(@"SOFTWARE\CHAIR");
        }

        public static void initializeSettings()
        {
            RegistryKey key = getRegistryKey();
            if(key.GetValue(SettingsConstants.InstallationFolder) == null)
                key.SetValue(SettingsConstants.InstallationFolder, getDefaultInstallingFolder());

            if(key.GetValue(SettingsConstants.TempFolder) == null)
                key.SetValue(SettingsConstants.TempFolder, getDefaultTempDownloadFolder());

            if(key.GetValue(SettingsConstants.NotifMessage) == null)
                key.SetValue(SettingsConstants.NotifMessage, true);

            if(key.GetValue(SettingsConstants.NotifOnline) == null)
                key.SetValue(SettingsConstants.NotifOnline, true);

            if(key.GetValue(SettingsConstants.NotifOffline) == null)
                key.SetValue(SettingsConstants.NotifOffline, true);

            if (key.GetValue(SettingsConstants.NotifPlayingGame) == null)
                key.SetValue(SettingsConstants.NotifPlayingGame, true);

            if (key.GetValue(SettingsConstants.UsernameRememberMe) == null)
                key.SetValue(SettingsConstants.UsernameRememberMe, "");

            if (key.GetValue(SettingsConstants.PasswordRememberMe) == null)
                key.SetValue(SettingsConstants.PasswordRememberMe, "");

            if (key.GetValue(SettingsConstants.MinimizeToTray) == null)
                key.SetValue(SettingsConstants.MinimizeToTray, true);

            key.Close();
        }

        #region Installing Folder
        public static string getDefaultInstallingFolder()
        {
            //Get the default folder path and add our own path
            string instFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            instFolder += "\\CHAIR\\Games";

            //This creates the directory if it doesn't exist. If it does, it doesn't do anything
            Directory.CreateDirectory(instFolder);

            return instFolder;
        }

        public static string getInstallingFolder()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the installation folder saved in the settings
            string instFolder = regKey.GetValue(SettingsConstants.InstallationFolder).ToString();

            //Create the directory just in case. If it exists, it does nothing
            Directory.CreateDirectory(instFolder);

            //Close the registry key
            regKey.Close();
            
            return instFolder;
        }

        public static void setInstallingFolder(string installingFolder)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.InstallationFolder, installingFolder);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Temp Folder
        public static string getDefaultTempDownloadFolder()
        {
            string tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            tempFolder += "\\CHAIR\\Temp";
            
            //This creates the directory if it doesn't exist. If it does, it doesn't do anything
            Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }

        public static string getTempDownloadFolder()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the installation folder saved in the settings
            string tempFolder = regKey.GetValue(SettingsConstants.TempFolder).ToString();

            //Create the directory just in case. If it exists, it does nothing
            Directory.CreateDirectory(tempFolder);

            //Close the registry key
            regKey.Close();
            
            return tempFolder;
        }

        public static void setTempDownloadFolder(string tempFolder)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.TempFolder, tempFolder);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Message Notifications
        public static bool getMessageNotificationSetting()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the setting
            bool notifMessage;
            bool success = bool.TryParse((string)regKey.GetValue(SettingsConstants.NotifMessage), out notifMessage);

            //Close the registry key
            regKey.Close();
            
            return notifMessage;
        }

        public static void setMessageNotificationSetting(bool notifMessage)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.NotifMessage, notifMessage);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Online Notifications
        public static bool getOnlineNotificationSetting()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the setting
            bool notifOnline;
            bool success = bool.TryParse((string)regKey.GetValue(SettingsConstants.NotifOnline), out notifOnline);

            //Close the registry key
            regKey.Close();
            
            return notifOnline;
        }

        public static void setOnlineNotificationSetting(bool notifOnline)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.NotifOnline, notifOnline);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Offline Notifications
        public static bool getOfflineNotificationSetting()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the setting
            bool notifOffline;
            bool success = bool.TryParse((string)regKey.GetValue(SettingsConstants.NotifOffline), out notifOffline);

            //Close the registry key
            regKey.Close();
            
            return notifOffline;
        }

        public static void setOfflineNotificationSetting(bool notifOffline)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.NotifOffline, notifOffline);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Playing Game Notifications
        public static bool getPlayingGameNotificationSetting()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the setting
            bool notifPlayingGame;
            bool success = bool.TryParse((string)regKey.GetValue(SettingsConstants.NotifPlayingGame), out notifPlayingGame);

            //Close the registry key
            regKey.Close();
            
            return notifPlayingGame;
        }

        public static void setPlayingGameNotificationSetting(bool notifPlayingGame)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.NotifPlayingGame, notifPlayingGame);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Username Remember Me
        public static string getUsernameRememberMe()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the setting
            string username = (string)regKey.GetValue(SettingsConstants.UsernameRememberMe);

            //Close the registry key
            regKey.Close();

            return username;
        }

        public static void setUsernameRememberMe(string username)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.UsernameRememberMe, username);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Password Remember Me
        public static string getPasswordRememberMe()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the setting
            string password = (string)regKey.GetValue(SettingsConstants.PasswordRememberMe);

            //Close the registry key
            regKey.Close();

            return password;
        }

        public static void setPasswordRememberMe(string password)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.PasswordRememberMe, password);

            //Close the registry key
            regKey.Close();
        }
        #endregion

        #region Playing Game Notifications
        public static bool getMinimizeToTraySetting()
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Get the setting
            bool minToTray;
            bool success = bool.TryParse((string)regKey.GetValue(SettingsConstants.MinimizeToTray), out minToTray);

            //Close the registry key
            regKey.Close();

            return minToTray;
        }

        public static void setMinimizeToTraySetting(bool minToTray)
        {
            //Open the registry key
            RegistryKey regKey = getRegistryKey();

            //Save the setting
            regKey.SetValue(SettingsConstants.MinimizeToTray, minToTray);

            //Close the registry key
            regKey.Close();
        }
        #endregion


    }
}
