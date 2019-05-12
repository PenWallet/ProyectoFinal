﻿using CHAIRSignalR_DAL.Connection;
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

namespace CHAIRSignalR_DAL
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
            else if(status == HttpStatusCode.Unauthorized && string.IsNullOrEmpty(response.Content))
                return JsonConvert.DeserializeObject<BanResponse>(response.Content);

            return null;
        }
    }
}
