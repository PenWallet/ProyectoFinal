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
    public static class MessageCallback
    {
        public static List<Message> getConversation(string me, string friend, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("messages/{user1}/with/{user2}", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Accept", "application/json");
            request.AddUrlSegment("user1", me);
            request.AddUrlSegment("user2", friend);

            //Make the request
            var response = APIConnection.Client.Execute<List<Message>>(request);

            //Profit
            status = response.StatusCode;

            if (status == HttpStatusCode.OK)
                return response.Data;

            return null;
        }

        public static void postMessage(Message message, string token, out HttpStatusCode status)
        {
            //Prepare the request
            RestRequest request = new RestRequest("messages", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(message);

            //Make the request
            var response = APIConnection.Client.Execute(request);

            //Profit
            status = response.StatusCode;
        }
    }
}
