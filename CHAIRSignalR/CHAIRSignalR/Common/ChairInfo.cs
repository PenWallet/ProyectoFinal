using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHAIRSignalR.Common
{
    public static class ChairInfo
    {
        //Dictionary for <nickname, ConnectionId>
        public static ConcurrentDictionary<string, string> onlineUsers { get; set; }

        //Dictionary for <nickname, KeyValuePair<game, fecha en la que empezó a jugar>>
        public static ConcurrentDictionary<string, KeyValuePair<string, DateTime>> usersPlaying { get; set; }

        //Dictionary for <nickname, ConnectionId>
        public static ConcurrentDictionary<string, string> onlineAdmins { get; set; }
    }
}