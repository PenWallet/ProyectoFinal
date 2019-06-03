using CHAIR_Entities.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.Utils
{
    /// <summary>
    /// This is most likely not the best solution for sharing information across ViewModels, but the other
    /// solutions are just too expensive time-wise
    /// </summary>
    public static class SharedInfo
    {
        public static UserWithToken loggedUser { get; set; } = new UserWithToken();
        public static string gameBeingDownloaded { get; set; }
        public static string gameBeingUnzipped { get; set; }
        public static string gameBeingPlayed { get; set; }
    }
}
