using RestSharp;
using System;
using System.Net;

namespace MediasiteUtil.Models
{
    public class Response<T>
    {
        /// <summary>
        /// Indicates whether the response succeeded according to expectations
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// Original request
        /// </summary>
        public string resource { get; set; }
        /// <summary>
        /// Status Code of http response
        /// </summary>
        public HttpStatusCode statusCode { get; set; }
        /// <summary>
        /// Description of status code
        /// </summary>
        public string statusDescription { get; set; }
        /// <summary>
        /// Error message returned in response
        /// </summary>
        public string errorMessage { get; set; }
        /// <summary>
        /// Content of response.
        /// </summary>
        public T content { get; set; }
        /// <summary>
        /// Inner response from a chain of requests
        /// </summary>
        public Response<T> InnerResponse { get; set; }
    }
}