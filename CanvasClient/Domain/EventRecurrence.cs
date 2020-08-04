using System;

namespace CanvasClient.Domain
{
    public class EventRecurrence
    {
        public EventRecurrence()
        {
            interval = 1;
            frequency = "weekly";
        }

        /// <summary>
        /// Number of times to duplicate event.  Cannot exceed 300.
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// The interval between the duplicated events.  Defaults to 1.
        /// </summary>
        public int interval { get; set; }
        /// <summary>
        /// The frequency at which to duplicate the event (default is "weekly", others "daily", "monthly")
        /// </summary>
        public string frequency { get; set; }
    }
}
