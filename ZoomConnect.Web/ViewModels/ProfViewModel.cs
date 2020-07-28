using System;
using System.Linq;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.ViewModels
{
    public class ProfViewModel
    {
        public ProfViewModel(ProfDataModel prof)
        {
            Name = $"{prof.bannerPerson.last_name}, {prof.bannerPerson.first_name}";
            Email = prof.primaryEmail == null ? "no email" : prof.primaryEmail.email_address;
            altEmailCount = prof.otherEmails.Count;
            altEmails = String.Join(", ", prof.otherEmails.Select(e => e.email_address));
            courseCount = prof.assignments.Count;
            courses = String.Join(", ", prof.assignments
                .OrderBy(a => a.crse_numb)
                .Select(a => a.crse_numb));
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public int altEmailCount { get; set; }
        public string altEmails { get; set; }
        public int courseCount { get; set; }
        public string courses { get; set; }
    }
}
