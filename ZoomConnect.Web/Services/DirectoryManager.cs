using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using System;
using System.IO;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Services
{
    public class DirectoryManager
    {
        private ZoomOptions _zoomOptions;
        private ILogger<DirectoryManager> _logger;

        public DirectoryManager(SecretConfigManager<ZoomOptions> optionsManager, ILogger<DirectoryManager> logger)
        {
            _zoomOptions = optionsManager.GetValue().Result;
            _logger = logger;
        }

        public string DownloadDirectory
        {
            get
            {
                var dir = _zoomOptions.DownloadDirectory;
                if (String.IsNullOrWhiteSpace(dir)) { return ""; }
                if (!Directory.Exists(dir))
                {
                    _logger.LogInformation($"Creating download directory: {dir}");
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        public string DownloadNonMp4Directory
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_zoomOptions.DownloadDirectory)) { return ""; }
                var dir = Path.Combine(DownloadDirectory, "non_mp4");
                if (!Directory.Exists(dir))
                {
                    _logger.LogInformation($"Creating download Non-MP4 directory: {dir}");
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        public string UploadDirectory
        {
            get
            {
                var dir = _zoomOptions.MediasiteOptions.UploadDirectory;
                if (String.IsNullOrWhiteSpace(dir)) { return ""; }
                if (!Directory.Exists(dir))
                {
                    _logger.LogInformation($"Creating upload directory: {dir}");
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        public string UploadOutDirectory
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_zoomOptions.MediasiteOptions.UploadDirectory)) { return ""; }
                var dir = Path.Combine(UploadDirectory, "out");
                if (!Directory.Exists(dir))
                {
                    _logger.LogInformation($"Creating Upload Out directory: {dir}");
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }
    }
}
