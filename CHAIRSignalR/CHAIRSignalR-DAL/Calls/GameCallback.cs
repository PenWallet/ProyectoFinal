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
    public static class GameCallback
    {
        /// <summary>
        /// Method used to get all the games in the store
        /// </summary>
        /// <returns></returns>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static List<Game> getAllStoreGames(string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("games", Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");

            //Make the request
            var response = APIConnection.Client.Execute<List<Game>>(request);

            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.OK)
                return response.Data;
            
            return null;
        }
    }
}
