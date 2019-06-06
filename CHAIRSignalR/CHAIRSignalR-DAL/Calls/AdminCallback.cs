
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
    public static class AdminCallback
    {
        /// <summary>
        /// Method used to ban an user from entering the application
        /// </summary>
        /// <param name="user">The user information to be banned</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void ban(User user, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/ban", Method.PATCH);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(user);

            //Make the request
            var response = APIConnection.Client.Execute(request);
            
            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to pardon an user from a ban
        /// </summary>
        /// <returns></returns>
        /// <param name="nickname">The name of the user to be pardoned</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void pardonUser(string nickname, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/pardonuser/{nickname}", Method.PATCH);
            request.AddUrlSegment("nickname", nickname);
            request.AddHeader("Authorization", $"Bearer {token}");

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to pardon an user and his IP from a ban
        /// </summary>
        /// <returns></returns>
        /// <param name="nickname">The name of the user to be pardoned</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void pardonUserAndIP(string nickname, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/pardonuserandip/{nickname}", Method.PATCH);
            request.AddUrlSegment("nickname", nickname);
            request.AddHeader("Authorization", $"Bearer {token}");

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to ban an user and his IP from entering the application
        /// </summary>
        /// <returns></returns>
        /// <param name="user">The user information to be banned</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void banUserAndIp(User user, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/banuserandip", Method.PATCH);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(user);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to change the game displayed in the front page of the store
        /// </summary>
        /// <returns></returns>
        /// <param name="game">The game to be set to the front page</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void changeFrontPageGame(string game, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/frontpage/{game}", Method.PATCH);
            request.AddUrlSegment("game", game);
            request.AddHeader("Authorization", $"Bearer {token}");

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to add a new game to the store
        /// </summary>
        /// <returns></returns>
        /// <param name="game">The game to be added</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void addNewGame(Game game, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/games", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(game);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to get all information about all the games in the store
        /// </summary>
        /// <returns></returns>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static List<GameBeingPlayed> getGamesStats(string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/gamesstats", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Accept", "application/json");

            //Make the request
            var response = APIConnection.Client.Execute<List<GameBeingPlayed>>(request);

            //Profit
            status = response.StatusCode;

            if(response.StatusCode == HttpStatusCode.OK)
                return response.Data;
            else
                return null;

        }

        /// <summary>
        /// Method used to geta list with the names of all the users in the application
        /// </summary>
        /// <returns></returns>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static List<string> getAllUsers(string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/users", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Accept", "application/json");

            //Make the request
            var response = APIConnection.Client.Execute<List<string>>(request);

            //Profit
            status = response.StatusCode;

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }

        /// <summary>
        /// Method used to get a list with the names of all the banned users in the application
        /// </summary>
        /// <returns></returns>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static List<string> getBannedUsers(string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/bannedusers", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Accept", "application/json");

            //Make the request
            var response = APIConnection.Client.Execute<List<string>>(request);

            //Profit
            status = response.StatusCode;

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }
    }
}
