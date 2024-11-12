#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System.Net.Http;
using System.Threading.Tasks;
using com.in3d.sdk.import;
using com.in3d.sdk.import.Web;
using UnityEngine;
using UnityEngine.Networking;

namespace com.in3d.sdk.loaders
{
    public static class LoadFromModelUrlsExtension
    {
        /// <summary> Loads preview of avatars model. </summary>
        /// <exception cref="HttpRequestException"> If preview cannot be loaded. </exception>
        public static async Task<Texture2D> LoadPreviewAsync(this ModelUrls urls)
        {
            var request = await Requests.Image(urls.PreviewUrl);
            if (!request.Success()) throw new HttpRequestException(request.error);
            return DownloadHandlerTexture.GetContent(request);
        }
    }
}

#endif