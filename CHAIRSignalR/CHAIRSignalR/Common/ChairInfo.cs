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
    }
}