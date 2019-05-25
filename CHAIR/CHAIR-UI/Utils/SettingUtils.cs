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
        public static string getTempDownloadFolder()
        {
            string tempFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            tempFolder += "\\CHAIR\\Temp";
            
            //This creates the directory if it doesn't exist. If it does, it doesn't do anyt
            Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }

        public static string getInstallingFolder()
        {
            string instFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            instFolder += "\\CHAIR\\Games";

            //This creates the directory if it doesn't exist. If it does, it doesn't do anyt
            Directory.CreateDirectory(instFolder);

            return instFolder;
        }

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
    }
}
