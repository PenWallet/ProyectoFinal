using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using CHAIRSignalR_DAL;
using CHAIRSignalR_Entities.Persistent;
using System.Net;
using CHAIRSignalR_Entities.Complex;

namespace CHAIRSignalR.Hubs
{
    public class LoginHub : Hub
    {
        public void login(string username, string password)
        {
            //Prepare the user
            User user = new User();
            user.nickname = username;
            user.password = password;
            user.lastIP = (string)Context.Request.Environment["server.RemoteIpAddress"];

            //Make the call to the API
            HttpStatusCode statusCode;
            object response = UserCallback.login(user, out statusCode);

            if (statusCode == HttpStatusCode.OK)
                Clients.Caller.loginSuccessful((UserWithToken)response);
        }



        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
        
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}