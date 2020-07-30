using System;
using RestSharp;

namespace CanvasClient
{
    public class CanvasApi
    {
        private readonly string BaseUrl = "https://canvas.ucdavis.edu/api/v1/";
        private RestClient client = null;

        public CanvasApi()
        {
            client = new RestClient(BaseUrl);
        }
    }
}
