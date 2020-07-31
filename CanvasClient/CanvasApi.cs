using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using CanvasClient.Domain;
using RestSharp;
using RestSharp.Authenticators;
using SecretJsonConfig;
using ZoomConnect.Core.Config;

namespace CanvasClient
{
    public class CanvasApi
    {
        private readonly string BaseUrl = "https://canvas.ucdavis.edu/api/v1/";
        private RestClient client = null;
        private CanvasApiOptions _canvasOptions;
        private int PageSize = 50;

        public CanvasApi(SecretConfigManager<ZoomOptions> options)
        {
            client = new RestClient(BaseUrl);
            _canvasOptions = options.GetValue().Result.CanvasApi;
        }

        /// <summary>
        /// A test call that gets only the first account from GET /accounts.
        /// Checks response to look for invalid token or other errors.
        /// </summary>
        /// <returns>null if test passed, error message otherwise.</returns>
        public string TokenErrorCheck()
        {
            client.Authenticator = ApiToken;

            var request = new RestRequest("accounts", Method.GET, DataFormat.Json)
                .AddParameter("per_page", 1);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var account = JsonSerializer.Deserialize<List<Account>>(response.Content);
                if (account == null || account.Count == 0)
                {
                    return "http status ok but no data returned.";
                }
                return null;
            }

            return response.StatusDescription;
        }

        private JwtAuthenticator ApiToken
        {
            get
            {
                return new JwtAuthenticator(_canvasOptions.ApiAccessToken);
            }
        }
    }
}
