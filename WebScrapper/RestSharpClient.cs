using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class RestSharpClient
    {
        public RestClient GetRestSharpClient(string url)
        {
            return new RestClient(url);
        }

        public RestRequest SetRequestType()
        {
            return new RestRequest(Method.Get.ToString());
        }

        public RestResponse ExecuteRequest(RestClient client, RestRequest request)
        {
            return client.Execute(request);
        }
    }
}
