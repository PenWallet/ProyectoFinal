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
    public static class GameStoreCallback
    {
        /// <summary>
        /// Method used to retrieve the information of a game as well as the relationship the given user has with that game
        /// </summary>
        /// <param name="nickname">The name of the user</param>
        /// <param name="game">The name of the game</param>
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
