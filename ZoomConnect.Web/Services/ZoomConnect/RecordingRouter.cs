using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoomClient.Domain;

namespace ZoomConnect.Web.Services.ZoomConnect
{
    /// <summary>
    /// Service that decides where to route downloaded Zoom recordings.
    /// By default this is the configured download directory.
    /// If specific criteria are met, send to upload directory instead.
    /// </summary>
    public class RecordingRouter
    {
        private DirectoryManager _directoryManager;

        public RecordingRouter(DirectoryManager directoryManager)
        {
            _directoryManager = directoryManager;
        }

        /// <summary>
        /// Returns directory to download Zoom recordings.
        /// If recording is 100MB+ and 45 minutes+, route directly to upload directory.
        /// Otherwise route to download directory.
        /// </summary>
        /// <param name="recording"></param>
        /// <returns></returns>
        public string GetDirectoryForRecording(Recording recording)
        {
            if (recording.file_size / (decimal)Math.Pow(2, 20) >= 100 &&
                recording.RecordingDurationMinutes >= 45)
            {
                return _directoryManager.UploadDirectory;
            }

            return _directoryManager.DownloadDirectory;
        }
    }
}
