using System;

namespace ZoomClient.Domain.Billing
{
    public class PlanUsage : ZObject
    {
        public PlanDetails plan_base { get; set; }
        public PlanRecording plan_recording { get; set; }
        public PlanDetails plan_webinar { get; set; }
        public PlanDetails plan_zoom_rooms { get; set; }
    }
}
