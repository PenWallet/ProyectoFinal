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
    public static class UserCallback
    {
        /// <summary>
        /// Method used to login
        /// </summary>
        /// <param name="user">The user who wants to log-in</param>
        /// <param name="status">Same as the API response</param>
        /// <returns></returns>
        public static object login(User user, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("users/login", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(user);

            //Make the request
            var response = APIConnection.Client.Execute(request);
            
            //Profit
            status = response.StatusCode;
            
            if (status == HttpStatusCode.OK)
            {
                User usr = JsonConvert.DeserializeObject<User>(response.Content);
                string token = ((string)response.Headers.Single(x => x.Name == "Authentication").Value).Split(' ')[1];
                return new UserWithToken(usr, token);
            }
            else if(status == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(response.Content))
                return JsonConvert.DeserializeObject<BanResponse>(response.Content);

            return null;
        }

        /// <summary>
        /// Method used to register a new user
        /// </summary>
        /// <returns></returns>
        /// <param name="user">The user to register</param>
        /// <param name="status">The API's response</param>
        public static object register(User user, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("users/register", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(user);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(response.Content))
                return JsonConvert.DeserializeObject<BanResponse>(response.Content);

            return null;
        }

        /// <summary>
        /// Method used to set an user's status to online
        /// </summary>
        /// <returns></returns>
        /// <param name="nickname">The nickname of the user we want to set to online</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void online(string nickname, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("users/{nickname}/online", Method.PATCH);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("nickname", nickname);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to set an user's status to offline
        /// </summary>
        /// <returns></returns>
        /// <param name="nickname">The nickname of the user we want to set to offline</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void offline(string nickname, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("users/{nickname}/offline", Method.PATCH);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddUrlSegment("nickname", nickname);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }

        /// <summary>
        /// Method used to update an user's information
        /// </summary>
        /// <returns></returns>
        /// <param name="user">The user with the information to update</param>
        /// <param name="token">The caller's token</param>
        /// <param name="status">The API's response</param>
        public static void update(User user, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("users", Method.PUT);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(user);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }
    }
}
