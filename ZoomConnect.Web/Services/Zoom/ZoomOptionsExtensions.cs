using ZoomClient;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Services.Zoom
{
    public static class ZoomOptionsExtensions
    {
        public static Options CreateZoomOptions (this ZoomApiOptions zoomApiOptions)
        {
            return new Options
            {
                ApiKey = zoomApiOptions.ApiKey,
                ApiSecret = zoomApiOptions.ApiSecret
            };
        }
    }
}
