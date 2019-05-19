using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIR_Entities.Persistent
{
    public class Game
    {
        public string name { get; set; }
        public string description { get; set; }
        public string developer { get; set; }
        public int minimumAge { get; set; }
        public DateTime releaseDate { get; set; }
        public string instructions { get; set; }
        public string downloadUrl { get; set; }
        public string storeImageUrl { get; set; }
        public string libraryImageUrl { get; set; }
        public bool frontPage { get; set; }

        public Game(string name, string description, string developer, int minimumAge, DateTime releaseDate, string instructions, string downloadUrl, string storeImageUrl, string libraryImageUrl, bool frontPage)
        {
            this.name = name;
            this.description = description;
            this.developer = developer;
            this.minimumAge = minimumAge;
            this.releaseDate = releaseDate;
            this.instructions = instructions;
            this.downloadUrl = downloadUrl;
            this.storeImageUrl = storeImageUrl;
            this.libraryImageUrl = libraryImageUrl;
            this.frontPage = frontPage;
        }

        public Game()
        {
        }
    }
}
