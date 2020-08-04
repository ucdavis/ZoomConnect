using System;

namespace CanvasClient.Domain
{
    /// <summary>
    /// Request to create a calendar event
    /// </summary>
    /// <remarks>https://canvas.instructure.com/doc/api/all_resources.html#method.calendar_events_api.create</remarks>
    public class CalendarEventRequest
    {
        public EventRequestData calendar_event { get; set; }
    }
}
