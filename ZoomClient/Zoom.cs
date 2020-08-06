using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using JWT.Algorithms;
using JWT.Builder;
using RestSharp;
using RestSharp.Authenticators;
using ZoomClient.Domain;
using ZoomClient.Extensions;
using System.IO;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomClient.Domain.Billing;
using Newtonsoft.Json;

namespace ZoomClient
{
    public class Zoom
    {
        private readonly string BaseUrl = "https://api.zoom.us/v2/";
        private RestClient client = null;
        private int PageSize = 80;
        private ZoomApiOptions _zoomOptions;

        public Zoom(SecretConfigManager<ZoomOptions> zoomOptions)
        {
            client = new RestClient(BaseUrl);
            _zoomOptions = zoomOptions.GetValue().Result.ZoomApi;
        }

        /// <summary>
        /// Gets a specific Zoom User by id (userid or email address)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/users/user</remarks>
        public User GetUser(string userId)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("users/{userId}", Method.GET, DataFormat.Json)
                .AddParameter("userId", userId, ParameterType.UrlSegment);

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<User>(response.Content);
            }

            return null;
        }

        /// <summary>
        /// Gets all Zoom users on this account.
        /// </summary>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/users/users</remarks>
        public List<User> GetUsers()
        {
            var page = 0;
            var pages = 1;
            List<User> users = new List<User>();

            do
            {
                page++;

                var request = new RestRequest("users", Method.GET, DataFormat.Json)
                    .AddParameter("status", "active")
                    .AddParameter("page_size", PageSize)
                    .AddParameter("page_number", page);

                client.Authenticator = NewToken;
                var response = client.Execute(request);
                Thread.Sleep(RateLimit.Medium);

                var result = JsonConvert.DeserializeObject<ZList<User>>(response.Content);
                if (result != null && result.Results != null)
                {
                    users.AddRange(result.Results);
                }

                pages = result.page_count;
            }
            while (page < pages);

            // TODO prep for deprecation of page_number in favor of next_page_token (see ZList)

            return users;
        }

        /// <summary>
        /// Create a new Zoom User as licensed user including name and email address
        /// </summary>
        /// <param name="userRequest">user request object with name and email address</param>
        /// <returns>UserInfo with new id included</returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/users/usercreate</remarks>
        public UserInfo CreateUser(UserRequest userRequest)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("users", Method.POST, DataFormat.Json)
                .AddJsonBody(userRequest);

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<UserInfo>(response.Content);
            }

            return null;
        }

        /// <summary>
        /// Update a Zoom User's profile
        /// </summary>
        /// <param name="userId">Id of user to update</param>
        /// <param name="profileChanges">Changes to user's profile.
        /// Only fill out properties you want changed, others will be ignored.</param>
        /// <returns>true if updated successfully</returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/users/userupdate</remarks>
        public bool UpdateUserProfile(string userId, UserUpdate profileChanges)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("users/{userId}", Method.PATCH, DataFormat.Json)
                .AddParameter("userId", userId)
                .AddJsonBody(profileChanges);

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            return (response.StatusCode == HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Gets details of a Zoom Meeting
        /// </summary>
        /// <param name="meetingId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meeting</remarks>
        public Meeting GetMeetingDetails(string meetingId)
        {
            return GetMeetingDetails(meetingId, null);
        }

        /// <summary>
        /// Gets details of a Zoom Meeting
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="occurrenceId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meeting</remarks>
        public Meeting GetMeetingDetails(string meetingId, string occurrenceId)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("/meetings/{meetingId}", Method.GET, DataFormat.Json)
                .AddParameter("meetingId", meetingId, ParameterType.UrlSegment);

            // TODO add new parameter:  bool show_previous_occurrences

            if (!String.IsNullOrEmpty(occurrenceId))
            {
                request.AddParameter("occurrence_id", occurrenceId, ParameterType.QueryString);
            }

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<Meeting>(response.Content);
            }

            return null;
        }

        /// <summary>
        /// All upcoming meetings for Zoom user by userid or email.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meetings</remarks>
        public List<Meeting> GetMeetingsForUser(string userId)
        {
            return GetMeetingsForUser(userId, "upcoming");
        }

        /// <summary>
        /// All meetings for Zoom user by meetingType and userId or email.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="meetingType"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meetings</remarks>
        public List<Meeting> GetMeetingsForUser(string userId, string meetingType)
        {
            var page = 0;
            var pages = 1;
            List<Meeting> meetings = new List<Meeting>();

            do
            {
                page++;

                var request = new RestRequest("users/{userId}/meetings", Method.GET, DataFormat.Json)
                    .AddParameter("userId", userId, ParameterType.UrlSegment)
                    .AddParameter("type", meetingType)
                    .AddParameter("page_size", PageSize)
                    .AddParameter("page_number", page);

                client.Authenticator = NewToken;
                var response = client.Execute(request);
                Thread.Sleep(RateLimit.Medium);

                var result = JsonConvert.DeserializeObject<ZList<Meeting>>(response.Content);
                if (result != null && result.Results != null)
                {
                    meetings.AddRange(result.Results);
                }

                pages = result.page_count;
            }
            while (page < pages);

            // TODO prep for deprecation of page_number in favor of next_page_token (see ZList)

            return meetings;
        }

        /// <summary>
        /// Get list of ended meeting instances by meeting id
        /// </summary>
        /// <param name="meetingId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/pastmeetings</remarks>
        public List<Meeting> GetPastMeetingInstances(string meetingId)
        {
            var request = new RestRequest("/past_meetings/{meetingId}/instances", Method.GET, DataFormat.Json)
                .AddParameter("meetingId", meetingId, ParameterType.UrlSegment);

            client.Authenticator = NewToken;
            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Medium);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            Console.Write(response.Content);

            var result = JsonConvert.DeserializeObject<ZList<Meeting>>(response.Content);
            return result.Results.ToList();
        }

        /// <summary>
        /// Get details of past meeting by UUID
        /// </summary>
        /// <param name="meetingUUID"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/pastmeetingdetails</remarks>
        public Meeting GetPastMeetingDetails(string meetingUUID)
        {
            var request = new RestRequest("/past_meetings/{meetingUUID}", Method.GET, DataFormat.Json)
                .AddParameter("meetingUUID", meetingUUID, ParameterType.UrlSegment);

            client.Authenticator = NewToken;
            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Meeting>(response.Content);
        }

        /// <summary>
        /// Create meeting for user
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meetingcreate</remarks>
        public Meeting CreateMeetingForUser(MeetingRequest meeting, string userId)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("users/{userId}/meetings", Method.POST, DataFormat.Json)
                .AddParameter("userId", userId, ParameterType.UrlSegment)
                .AddJsonBody(meeting);

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Medium);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<Meeting>(response.Content);
            }

            return null;
        }

        /// <summary>
        /// End a meeting by meeting id.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meetingstatus</remarks>
        public bool EndMeeting(string meetingId)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("meetings/{meetingId}/status", Method.PUT, DataFormat.Json)
                .AddParameter("meetingId", meetingId, ParameterType.UrlSegment)
                .AddJsonBody(new EndAction());

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Deletes a meeting by meeting id
        /// </summary>
        /// <param name="meetingId">Id of meeting to delete</param>
        /// <param name="sendReminder">True to send reminder to hosts about deletion.  Defaults to false.</param>
        /// <returns>true if deleted successfully</returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meetingdelete</remarks>
        public bool DeleteMeeting(string meetingId, bool sendReminder = false)
        {
            return DeleteMeeting(meetingId, null, sendReminder);
        }

        /// <summary>
        /// Deletes a meeting by meeting id and occurrenceId
        /// </summary>
        /// <param name="meetingId">Id of meeting to delete</param>
        /// <param name="occurrenceId">Id of occurrence to delete.  If null or blank, entire meeting will be deleted.</param>
        /// <param name="sendReminder">True to send reminder to hosts about deletion.  Defaults to false.</param>
        /// <returns>true if deleted successfully</returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/meetings/meetingdelete</remarks>
        public bool DeleteMeeting(string meetingId, string occurrenceId, bool sendReminder = false)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("meetings/{meetingId}", Method.DELETE, DataFormat.Json)
                .AddParameter("meetingId", meetingId, ParameterType.UrlSegment)
                .AddParameter("schedule_for_reminder", sendReminder, ParameterType.QueryString);

            if (!String.IsNullOrEmpty(occurrenceId))
            {
                request = request.AddParameter("occurrence_id", occurrenceId, ParameterType.QueryString);
            }

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Gets recordings for the given user in the last 3 months.
        /// Stops after one page of results for now.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/cloud-recording/recordingslist</remarks>
        public List<Meeting> GetCloudRecordingsForUser(string userId)
        {
            var page = 0;
            var pages = 1;
            List<Meeting> meetings = new List<Meeting>();

            do
            {
                page++;

                var request = new RestRequest("users/{userId}/recordings", Method.GET, DataFormat.Json)
                    .AddParameter("userId", userId, ParameterType.UrlSegment)
                    .AddParameter("page_size", PageSize)
                    .AddParameter("page_number", page)
                    .AddParameter("from", DateTime.Now.AddMonths(-3).ToZoomUTCFormat())
                    .AddParameter("to", DateTime.Now.ToZoomUTCFormat());

                client.Authenticator = NewToken;
                var response = client.Execute(request);
                Thread.Sleep(RateLimit.Medium);

                var result = JsonConvert.DeserializeObject<ZList<Meeting>>(response.Content);
                if (result != null && result.Results != null)
                {
                    meetings.AddRange(result.Results);
                }

                pages = result.page_count;
                page = pages;           // stop after one page for now
            }
            while (page < pages);

            // TODO prep for deprecation of page_number in favor of next_page_token (see ZList)

            return meetings;
        }

        /// <summary>
        /// Downloads zoom cloud recording from url, using memory instead of stream.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/cloud-recording/recordingget</remarks>
        public IRestResponse DownloadRecording(string url)
        {
            var downloadClient = new RestClient("https://api.zoom.us/");

            var request = new RestRequest(url, Method.GET)
                .AddParameter("access_token", NewTokenString, ParameterType.QueryString);

            return downloadClient.Execute(request);
        }

        /// <summary>
        /// Downloads zoom cloud recording from url, using stream instead of memory (preferred).
        /// </summary>
        /// <param name="url"></param>
        /// <param name="saveToPath"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/cloud-recording/recordingget</remarks>
        public IRestResponse DownloadRecordingStream(string url, string saveToPath)
        {
            var downloadClient = new RestClient("https://api.zoom.us/");

            using (var writer = new FileStream(saveToPath, FileMode.Create, FileAccess.Write, FileShare.None, 128000, false))
            {
                using (var reader = new MemoryStream())
                {
                    var request = new RestRequest(url, Method.GET)
                        .AddParameter("access_token", NewTokenString, ParameterType.QueryString);

                    request.ResponseWriter = responseStream =>
                    {
                        using (responseStream)
                        {
                            responseStream.CopyTo(writer);
                        }
                    };

                    return downloadClient.Execute(request);
                }
            }
        }

        /// <summary>
        /// Deletes a specific recording file by meeting id and recording id.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="recordingId"></param>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/cloud-recording/recordingdeleteone</remarks>
        public bool DeleteRecording(string meetingId, string recordingId)
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("/meetings/{meetingId}/recordings/{recordingId}", Method.DELETE, DataFormat.Json)
                .AddParameter("meetingId", meetingId, ParameterType.UrlSegment)
                .AddParameter("recordingId", recordingId, ParameterType.UrlSegment)
                .AddParameter("action", "trash", ParameterType.QueryString);

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Light);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Gets a Plan Usage report for the entire account
        /// </summary>
        /// <returns></returns>
        /// <remarks>https://marketplace.zoom.us/docs/api-reference/zoom-api/billing/getplanusage</remarks>
        public PlanUsage GetPlanUsage()
        {
            client.Authenticator = NewToken;

            var request = new RestRequest("/accounts/{accountId}/plans/usage", Method.GET, DataFormat.Json)
                .AddParameter("accountId", "me", ParameterType.UrlSegment);

            var response = client.Execute(request);
            Thread.Sleep(RateLimit.Heavy);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<PlanUsage>(response.Content);
            }

            return null;
        }

        private JwtAuthenticator NewToken
        {
            get
            {
                return new JwtAuthenticator(NewTokenString);
            }
        }

        private string NewTokenString
        {
            get
            {
                return new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(_zoomOptions.ApiSecret.Value)
                    .AddClaim("iss", _zoomOptions.ApiKey.Value)
                    .AddClaim("exp", DateTimeOffset.UtcNow.AddSeconds(5).ToUnixTimeSeconds())
                    .Encode();
            }
        }
    }
}
