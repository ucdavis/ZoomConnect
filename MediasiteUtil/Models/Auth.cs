using System;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace MediasiteUtil.Models
{
	public class Auth : IAuthenticator
	{
		private string userName, password, apiKey;

		public Auth(string userName, string password, string apiKey)
		{
			this.userName = userName;
			this.password = password;
			this.apiKey = apiKey;
		}

		public void Authenticate(IRestClient client, IRestRequest request)
		{
			var basicAuthHeaderValue = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.userName, password))));
			request.AddHeader("Authorization", basicAuthHeaderValue);
			request.AddHeader("sfapikey", this.apiKey);
		}
	}
}