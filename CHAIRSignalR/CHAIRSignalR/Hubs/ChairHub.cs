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
using CHAIRSignalR_Entities.Responses;
using System.Threading;
using CHAIRSignalR.Common;
using CHAIRSignalR_DAL.Calls;

namespace CHAIRSignalR.Hubs
{
    public class ChairHub : Hub
    {
        public void getAllStoreGames(string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            List<Game> games = GameCallback.getAllStoreGames(token, out statusCode);

            if (games != null)
                Clients.Caller.getAllStoreGames(games);
            else
                Clients.Caller.unexpectedError();

        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //Remove the user from the online list
            string nickname = ChairInfo.onlineUsers.Single(x => x.Value == Context.ConnectionId).Key;
            string conId;
            ChairInfo.onlineUsers.TryRemove(nickname, out conId);

            return base.OnDisconnected(stopCalled);
        }
        
        public override Task OnConnected()
        {
            //Add the user from the online list
            string nickname = Context.QueryString["nickname"];
            ChairInfo.onlineUsers.AddOrUpdate(nickname, Context.ConnectionId, (key, value) => value);

            return base.OnConnected();
        }
    }
}