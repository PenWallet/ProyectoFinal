using CHAIR_UI.ViewModels;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.SignalR
{
    public static class SignalRHubsConnection
    {
        private static string url = "http://localhost:51930/";
        private static SignalRConnection _loginHub { get; set; }
        private static SignalRConnection _chairHub { get; set; }

        public static SignalRConnection loginHub
        {
            get
            {
                if (_loginHub == null || _loginHub.conn.State == ConnectionState.Disconnected)
                {
                    _loginHub = new SignalRConnection();
                    _loginHub.conn = new HubConnection(url);
                    _loginHub.proxy = _loginHub.conn.CreateHubProxy("LoginHub");
                    _loginHub.conn.Start();
                }

                return _loginHub;
            }
        }

        public static SignalRConnection chairHub
        {
            get
            {
                if (_chairHub == null || _chairHub.conn.State == ConnectionState.Disconnected)
                {
                    _chairHub = new SignalRConnection();
                    _chairHub.conn = new HubConnection(url, $"nickname={SharedInfo.loggedUser.nickname}");
                    _chairHub.proxy = _chairHub.conn.CreateHubProxy("ChairHub");
                    _chairHub.conn.Start();
                }

                return _chairHub;
            }
        }
    }
}
