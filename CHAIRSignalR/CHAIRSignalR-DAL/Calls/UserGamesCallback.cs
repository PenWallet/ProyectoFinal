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
using CHAIRSignalR_Entidades.Complex;

namespace CHAIRSignalR_DAL.Calls
{
    public static class UserGamesCallback
    {
        /// <summary>
        /// Method used to retrieve all the games the specified user plays, along with the information of each game
        /// and which friends play each game
        /// </summary>
        /// <param name="nickname">The user who wants to get all his games</param>
        /// <param name="token">The user's token</param>
        /// <param name="status">Same as the API response</param>
        /// <returns></returns>
        public static List<UserGamesWithGameAndFriends> getAllMyGames(string nickname, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("usergamesinfo/{nickname}", Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("nickname", nickname);

            //Make the request
            var response = APIConnection.Client.Execute<List<UserGamesWithGameAndFriends>>(request);
            
            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.OK)
                return response.Data;

            return null;
        }
    }
}
