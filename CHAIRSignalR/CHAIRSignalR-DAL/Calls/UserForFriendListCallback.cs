using CHAIRSignalR_DAL.Connection;
using CHAIRSignalR_Entities.Complex;
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
    public static class UserForFriendListCallback
    {

        /// <summary>
        /// Method used to get all the friends an user has
        /// </summary>
        /// <returns></returns>
        /// <param name="username">The user of whom we want all his friends</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static List<UserForFriendList> getFriends(string username, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("userfriends/{username}", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Accept", "application/json");
            request.AddUrlSegment("username", username);

            //Make the request
            var response = APIConnection.Client.Execute<List<UserForFriendList>>(request);

            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.OK)
                return response.Data;

            return null;
        }
    }
}
