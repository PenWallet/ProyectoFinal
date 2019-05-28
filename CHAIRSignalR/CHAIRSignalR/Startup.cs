using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using System;
using CHAIRSignalR.Common;
using System.Collections.Concurrent;

[assembly: OwinStartup(typeof(CHAIRSignalR.Startup))]

namespace CHAIRSignalR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            //Initializing data structures
            ChairInfo.onlineUsers = new ConcurrentDictionary<string, string>();
            ChairInfo.usersPlaying = new ConcurrentDictionary<string, KeyValuePair<string, DateTime>>();
            ChairInfo.onlineAdmins = new ConcurrentDictionary<string, string>();

            //Marcar un usuario como desconectado tras 5 segundos
            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(6);
        }
    }
}