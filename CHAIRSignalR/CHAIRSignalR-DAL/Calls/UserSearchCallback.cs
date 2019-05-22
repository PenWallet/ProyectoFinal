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
    public static class UserSearchCallback
    {
        /// <summary>
        /// Method used to retrieve all the users who match contain the search parameter in their nicknames, as well
        /// as the date since the searching user and each user are friends
        /// </summary>
        /// <param name="search">The search parameter</param>
        /// <param name="token">The user's token</param>
        /// <param name="status">Same as the API response</param>
        /// <returns></returns>
        public static List<UserSearch> getSearchResults(string search, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("usersearch", Method.GET);
            request.AddParameter("s", search);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");

            //Make the request
            var response = APIConnection.Client.Execute<List<UserSearch>>(request);
            
            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.OK)
                return response.Data;

            return null;
        }
    }
}
