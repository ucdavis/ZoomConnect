using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using CanvasClient.Domain;
using CanvasClient.Extensions;
using RestSharp;
using RestSharp.Authenticators;

namespace CanvasClient
{
    public class CanvasApi
    {
        private readonly string BaseUrl = "https://canvas.ucdavis.edu/api/v1/";
        private RestClient client = null;
        private CanvasOptions _canvasOptions;
        private int PageSize = 50;

        public CanvasApi()
        {
            client = new RestClient(BaseUrl);
        }

        public CanvasApi(CanvasOptions options)
        {
            client = new RestClient(BaseUrl);
            _canvasOptions = options;
        }

        public CanvasOptions Options
        {
            set
            {
                _canvasOptions = value;
            }
        }

        /// <summary>
        /// A test call that gets only the first account from GET /accounts.
        /// Checks response to look for expired token or other errors.
        /// </summary>
        /// <returns>null if test passed and data was returned, error message otherwise.</returns>
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
                    return "Http status ok but no data returned";
                }
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (response.Headers.Any(h => h.Name.ToLower() == "www-authenticate"))
                {
                    return "Access Token expired or deleted";
                }
                else
                {
                    return "Unauthorized";
                }
            }

            return response.StatusDescription;
        }

        /// <summary>
        /// List Accounts
        /// </summary>
        /// <returns></returns>
        /// <remarks>https://canvas.instructure.com/doc/api/all_resources.html#method.accounts.index</remarks>
        public List<Account> ListAccounts()
        {
            client.Authenticator = ApiToken;

            var pagedData = new List<Account>();

            var request = new RestRequest("accounts", Method.GET, DataFormat.Json)
                .AddParameter("per_page", PageSize);

            do
            {
                var response = client.Get<List<Account>>(request);
                if (response.Data != null)
                {
                    pagedData.AddRange(response.Data);
                }

                request = new RestRequest(response.Headers.NextPageUrl());
            }
            while (!String.IsNullOrEmpty(request.Resource));

            return pagedData;
        }

        /// <summary>
        /// List Enrollment Terms for root account
        /// </summary>
        /// <returns></returns>
        /// <remarks>https://canvas.instructure.com/doc/api/all_resources.html#method.terms_api.index</remarks>
        public List<EnrollmentTerm> ListEnrollmentTerms()
        {
            client.Authenticator = ApiToken;

            var pagedData = new List<EnrollmentTerm>();

            var request = new RestRequest("accounts/{account_id}/terms", Method.GET, DataFormat.Json)
                .AddParameter("account_id", 1, ParameterType.UrlSegment)
                .AddParameter("per_page", PageSize);

            do
            {
                var response = client.Get<EnrollmentTermList>(request);
                if (response.Data != null)
                {
                    pagedData.AddRange(response.Data.enrollment_terms);
                }

                request = new RestRequest(response.Headers.NextPageUrl());
            }
            while (!String.IsNullOrEmpty(request.Resource));

            return pagedData;
        }

        /// <summary>
        /// List active courses in the given account and term
        /// </summary>
        /// <param name="accountId">Id of account to search.</param>
        /// <param name="termId">Id of term to search.</param>
        /// <returns></returns>
        /// <remarks>https://canvas.instructure.com/doc/api/all_resources.html#method.accounts.courses_api</remarks>
        public List<Course> ListActiveCourses(int accountId, int termId)
        {
            client.Authenticator = ApiToken;

            var pagedData = new List<Course>();

            var request = new RestRequest("accounts/{account_id}/courses", Method.GET, DataFormat.Json)
                .AddParameter("account_id", accountId, ParameterType.UrlSegment)
                .AddParameter("enrollment_term_id", termId)
                .AddParameter("with_enrollments", true)
                .AddParameter("per_page", PageSize);

            do
            {
                var response = client.Get<List<Course>>(request);
                if (response.Data != null)
                {
                    pagedData.AddRange(response.Data);
                }

                request = new RestRequest(response.Headers.NextPageUrl());
            }
            while (!String.IsNullOrEmpty(request.Resource));

            return pagedData;
        }

        /// <summary>
        /// List calendar events for a given course id, start date, and end date
        /// </summary>
        /// <param name="courseId">Id of course to list events for</param>
        /// <param name="startDate">Start of range to list events for</param>
        /// <param name="endDate">End of range to list events for</param>
        /// <returns>List of CalendarEvents matching course and date range.</returns>
        /// <remarks>https://canvas.instructure.com/doc/api/all_resources.html#method.calendar_events_api.index</remarks>
        public List<CalendarEvent> ListCalendarEvents(int courseId, DateTime startDate, DateTime endDate)
        {
            client.Authenticator = ApiToken;

            var pagedData = new List<CalendarEvent>();

            var request = new RestRequest("calendar_events", Method.GET, DataFormat.Json)
                .AddParameter("context_codes[]", $"course_{courseId}")
                .AddParameter("start_date", startDate.ToString("yyyy-MM-dd"))
                .AddParameter("end_date", endDate.ToString("yyyy-MM-dd"))
                .AddParameter("per_page", PageSize);

            do
            {
                var response = client.Get<List<CalendarEvent>>(request);
                if (response.Data != null)
                {
                    pagedData.AddRange(response.Data);
                }

                request = new RestRequest(response.Headers.NextPageUrl());
            }
            while (!String.IsNullOrEmpty(request.Resource));

            return pagedData;
        }

        /// <summary>
        /// Create a calendar event in a particular course
        /// </summary>
        /// <param name="eventRequest"></param>
        /// <returns>Created CalendarEvent, otherwise null</returns>
        public CalendarEvent CreateCalendarEvent(CalendarEventRequest eventRequest)
        {
            client.Authenticator = ApiToken;

            var request = new RestRequest("calendar_events", Method.POST, DataFormat.Json)
                .AddJsonBody(eventRequest);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.Created)
            {
               return null;
            }

            return JsonSerializer.Deserialize<CalendarEvent>(response.Content);
        }

        /// <summary>
        /// Delete a calendar event by id
        /// </summary>
        /// <param name="eventId">Id of CalendarEvent to delete</param>
        /// <returns>true if deleted successfully</returns>
        public bool DeleteCalendarEvent(int eventId)
        {
            client.Authenticator = ApiToken;

            var request = new RestRequest("calendar_events/{id}", Method.DELETE, DataFormat.Json)
                .AddParameter("id", eventId, ParameterType.UrlSegment);

            var response = client.Execute(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        private JwtAuthenticator ApiToken
        {
            get
            {
                if (_canvasOptions == null)
                {
                    throw new ArgumentException("Options not configured.");
                }
                return new JwtAuthenticator(_canvasOptions.ApiAccessToken);
            }
        }
    }
}
