using Microsoft.Extensions.DependencyInjection;
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
                AccountId = zoomApiOptions.AccountId,
                ClientId = zoomApiOptions.ClientId,
                ClientSecret = zoomApiOptions.ClientSecret
            };
        }
    }
}
