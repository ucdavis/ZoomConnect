using SecretJsonConfig;
using System;

namespace ZoomConnect.Core.Config
{
    public class CmdKeyOptions
    {
        /// <summary>
        /// Secret key allowing access to CmdController actions
        /// </summary>
        public SecretStruct CmdKey { get; set; }

        /// <summary>
        /// User who created the current CmdKey
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Date of last CmdKey update
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
