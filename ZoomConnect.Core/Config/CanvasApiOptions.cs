using System;
using SecretJsonConfig;

namespace ZoomConnect.Core.Config
{
    public class CanvasApiOptions
    {
        public CanvasApiOptions()
        {
            ApiAccessToken = new SecretStruct("");
        }

        /// <summary>
        /// Whether to apply changes to Canvas
        /// </summary>
        public bool UseCanvas { get; set; }
        /// <summary>
        /// Canvas API Access Token provided by user.
        /// </summary>
        public SecretStruct ApiAccessToken { get; set; }
        /// <summary>
        /// Id of selected Canvas Account.  Only courses in this account will be affected.
        /// </summary>
        public int SelectedAccount { get; set; }
        /// <summary>
        /// Id of Canvas Enrollment Term matching selected Banner Term.  Not shown to user.
        /// </summary>
        public int EnrollmentTerm { get; set; }
    }
}
