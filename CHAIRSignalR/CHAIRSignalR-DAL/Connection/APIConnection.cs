using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIRSignalR_DAL.Connection
{
    public static class APIConnection
    {
        //public static RestClient Client = new RestClient("https://pennychairapi.azurewebsites.net/");
        public static RestClient Client = new RestClient("https://localhost:44391");
    }
}
