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

        /// <summary>
        /// Method used to create a new friendship
        /// </summary>
        /// <returns></returns>
        /// <param name="user1">The user who sent the friend request</param>
        /// <param name="user2">The receiver of the friend request</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
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

        /// <summary>
        /// Method used to accept a friend request
        /// </summary>
        /// <returns></returns>
        /// <param name="user1">One of the users in the friendship</param>
        /// <param name="user2">One of the users in the friendship</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
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
        
        /// <summary>
        /// Method used to delete a friendship
        /// </summary>
        /// <returns></returns>
        /// <param name="user1">One of the users in the friendship</param>
        /// <param name="user2">One of the users in the friendship</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
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
