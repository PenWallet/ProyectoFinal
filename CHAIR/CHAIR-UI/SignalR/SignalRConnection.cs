using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.SignalR
{
    public class SignalRConnection
    {
        public HubConnection conn { get; set; }
        public IHubProxy proxy { get; set; }

        public SignalRConnection()
        {
        }

        public SignalRConnection(HubConnection conn, IHubProxy proxy)
        {
            this.conn = conn;
            this.proxy = proxy;
        }
    }
}
