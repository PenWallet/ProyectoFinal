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

        /// <summary>
        /// Method used to change an user's status to playing
        /// </summary>
        /// <returns></returns>
        /// <param name="user">The name of the user to be set to playing</param>
        /// <param name="game">The name of the game the user began to play</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
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

        /// <summary>
        /// Method used to change an user's status to not playing
        /// </summary>
        /// <returns></returns>
        /// <param name="user">The name of the user to be set to not playing</param>
        /// <param name="game">The name of the game the user stopped playing</param>
        /// <param name="secondsToAdd">The amount of time the user played the game for, in seconds</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
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

        /// <summary>
        /// Method used to add a game to an user's library
        /// </summary>
        /// <returns></returns>
        /// <param name="relationship">The new relationship between an user and a game</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
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
