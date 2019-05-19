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
    public static class GameStoreCallback
    {
        /// <summary>
        /// Method used to retrieve all the necessary information to display an user's profile
        /// </summary>
        /// <param name="nickname">The user from whom we want his profile</param>
        /// <param name="token">The user's token</param>
        /// <param name="status">Same as the API response</param>
        /// <returns></returns>
        public static GameStore getGameAndRelationship(string nickname, string game, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("gamestore/{game}/{nickname}", Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("nickname", nickname);
            request.AddUrlSegment("game", game);

            //Make the request
            var response = APIConnection.Client.Execute<GameStore>(request);
            
            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.OK)
                return response.Data;

            return null;
        }
    }
}
