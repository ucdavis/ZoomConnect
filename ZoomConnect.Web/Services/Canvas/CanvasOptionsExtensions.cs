using CanvasClient.Domain;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Services.Canvas
{
    public static class CanvasOptionsExtensions
    {
        public static CanvasOptions CreateCanvasOptions (this CanvasApiOptions canvasApiOptions)
        {
            return new CanvasOptions
            {
                ApiAccessToken = canvasApiOptions.ApiAccessToken
            };
        }
    }
}
