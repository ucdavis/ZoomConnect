using System;
using System.Threading.Tasks;
using RestSharp;

namespace MediasiteUtil
{
	/// <remarks>
	/// Stolen / modified from http://ianobermiller.com/blog/2012/07/23/restsharp-extensions-returning-tasks/
	/// </remarks>
	public static class RestClientExtensions
	{
		public static Task<IRestResponse<T>> GetAsyncResponse<T>(this RestClient client, IRestRequest request) where T : new()
		{
			var tcs = new TaskCompletionSource<IRestResponse<T>>();
			var loginResponse = client.ExecuteAsync<T>(request, r =>
			{
				if (r.ErrorException == null)
				{
					tcs.SetResult(r);
				}
				else
				{
					tcs.SetException(r.ErrorException);
				}
			});
			return tcs.Task;
		}

		public static Task<IRestResponse> GetAsyncResponse(this RestClient client, IRestRequest request)
		{
			var tcs = new TaskCompletionSource<IRestResponse>();
			var loginResponse = client.ExecuteAsync(request, r =>
			{
				if (r.ErrorException == null)
				{
					tcs.SetResult(r);
				}
				else
				{
					tcs.SetException(r.ErrorException);
				}
			});
			return tcs.Task;
		}
	}
}
