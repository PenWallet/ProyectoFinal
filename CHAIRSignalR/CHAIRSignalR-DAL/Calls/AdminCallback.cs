
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
        /// <returns></returns>
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

        public static void pardon(string nickname, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("admin/pardon/{nickname}", Method.PATCH);
            request.AddHeader("Authorization", $"Bearer {token}");

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }
    }
}
