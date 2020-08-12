using System;
using System.Collections.Generic;
using ZoomClient.Domain;

namespace ZoomConnect.Web.Models
{
    public class ParticipantReportModel
    {
        public string meetingId { get; set; }
        public string instanceId { get; set; }
        public string hostEmail { get; set; }
        public string subject { get; set; }
        public List<Participant> participants { get; set; }
    }
}
