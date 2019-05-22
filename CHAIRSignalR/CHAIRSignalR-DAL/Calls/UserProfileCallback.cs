using CHAIRSignalR_DAL.Connection;
using CHAIRSignalR_Entities.Persistent;
using CHAIRSignalR_Entities.Complex;
using CHAIRSignalR_Entities.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CHAIRSignalR_DAL.Calls
{
    public static class UserProfileCallback
    {
        /// <summary>
        /// Method used to retrieve all the necessary information to display an user's profile
        /// </summary>
        /// <param name="nickname">The user from whom we want his profile</param>
        /// <param name="token">The user's token</param>
        /// <param name="status">Same as the API response</param>
        /// <returns></returns>
        public static UserProfile getUserProfile(string nickname, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("userprofile/{nickname}", Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("nickname", nickname);

            //Make the request
            var response = APIConnection.Client.Execute<UserProfile>(request);
            
            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.OK)
                return response.Data;

            return null;
        }
    }
}
