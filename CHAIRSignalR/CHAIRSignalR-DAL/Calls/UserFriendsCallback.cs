using CHAIRSignalR_DAL.Connection;
using CHAIRSignalR_Entities.Persistent;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CHAIRSignalR_DAL.Calls
{
    public static class UserFriendsCallback
    {
        public static void saveNewRelationship(string user1, string user2, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("userfriends/{user1}/befriends/{user2}", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("user1", user1);
            request.AddUrlSegment("user2", user2);

            //Make the request
            var response = APIConnection.Client.Execute<List<Game>>(request);

            //Profit
            status = response.StatusCode;
        }

        public static void acceptFriendship(string user1, string user2, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("userfriends/{user1}/accept/{user2}", Method.PATCH);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("user1", user1);
            request.AddUrlSegment("user2", user2);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }
        
        public static void endFriendship(string user1, string user2, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("userfriends/{user1}/breakwith/{user2}", Method.DELETE);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("user1", user1);
            request.AddUrlSegment("user2", user2);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }
    }
}
