using SecretJsonConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Services.Zoom
{
    /// <summary>
    /// Finds Zoom User associated with each email address of an assigned prof.
    /// </summary>
    public class ZoomStudentFinder
    {
        private ZoomClient.Zoom _zoomClient;
        private CachedRepository<spriden_student> _studentRepo;
        private List<StudentDataModel> _students { get; set; }

        public ZoomStudentFinder(ZoomClient.Zoom zoomClient, CachedRepository<spriden_student> studentRepo, SecretConfigManager<ZoomOptions> optionsManager)
        {
            _zoomClient = zoomClient;
            _studentRepo = studentRepo;

            var _options = optionsManager.GetValue().Result;
            _zoomClient.Options = _options.ZoomApi.CreateZoomOptions();
        }

        /// <summary>
        /// Students linked to Zoom users by email
        /// </summary>
        public List<StudentDataModel> Students
        {
            get
            {
                if (_students == null)
                {
                    Find();
                }

                return _students;
            }
        }

        /// <summary>
        /// Searches for Zoom User by each email in spriden_students.
        /// </summary>
        /// <returns></returns>
        private void Find()
        {
            _students = new List<StudentDataModel>();
            var _allFound = new List<spriden_student>();

            // get all student rows
            var allStudents = _studentRepo.GetAll();

            // get all zoom accounts to a dictionary by email (filter out dupes)
            var zoomAccountsByEmail = _zoomClient.GetUsers("active")
                .ToDictionary(u => u.email);
            var pending = _zoomClient.GetUsers("pending")
                .Where(p => !zoomAccountsByEmail.ContainsKey(p.email))
                .ToList();
            pending.ForEach(p => { zoomAccountsByEmail.Add(p.email, p); });
            var inactive = _zoomClient.GetUsers("inactive")
                .Where(i => !zoomAccountsByEmail.ContainsKey(i.email))
                .ToList();
            inactive.ForEach(i => { zoomAccountsByEmail.Add(i.email, i); });

            // put each student in a model with its zoom account if found
            _students = allStudents.Select(s => new StudentDataModel { bannerPerson = s })
                .ToList();

            _students.ForEach(s =>
            {
                if (s.bannerPerson.email != null && zoomAccountsByEmail.ContainsKey(s.bannerPerson.email))
                {
                    s.zoomUser = zoomAccountsByEmail[s.bannerPerson.email];
                }
            });
        }
    }
}
