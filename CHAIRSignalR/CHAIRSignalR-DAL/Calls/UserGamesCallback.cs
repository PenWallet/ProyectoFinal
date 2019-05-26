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

        public static void setPlayingTrue(string user, string game, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("usergames/{user}/playing/{game}", Method.PATCH);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("user", user);
            request.AddUrlSegment("game", game);

            //Make the request
            var response = APIConnection.Client.Execute(request);
            
            //Profit
            status = response.StatusCode;
        }

        public static void setPlayingFalse(string user, string game, int secondsToAdd, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("usergames/{user}/notplaying/{game}", Method.PATCH);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("user", user);
            request.AddUrlSegment("game", game);
            request.AddQueryParameter("s", secondsToAdd.ToString());

            //Make the request
            var response = APIConnection.Client.Execute(request);
            
            //Profit
            status = response.StatusCode;
        }

        public static void buyGame(UserGames relationship, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("usergames", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(relationship);

            //Make the request
            var response = APIConnection.Client.Execute(request);
            
            //Profit
            status = response.StatusCode;
        }
    }
}
