using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace com.in3d.sdk.import.Web
{
    [DebuggerNonUserCode]
    public readonly struct UnityWebRequestAwaiter : INotifyCompletion
    {
        private readonly UnityWebRequestAsyncOperation _asyncOperation;

        public bool IsCompleted => _asyncOperation.isDone;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOperation) => _asyncOperation = asyncOperation;

        public void OnCompleted(Action continuation) => _asyncOperation.completed += _ => continuation();

        public UnityWebRequest GetResult() => _asyncOperation.webRequest;
    }

    public static class Requests
    {
        // TODO Protect from same requests: static Dictionary<string, Task<UnityWebRequest>>

        /// <summary> Is request succeed? </summary>
        /// <param name="request"></param>
        /// <returns>true if success</returns>
        public static bool Success(this UnityWebRequest request)
        {
#if UNITY_2020_1_OR_NEWER
            return request.isDone && request.result == UnityWebRequest.Result.Success;
#else
        return request.isDone && !request.isNetworkError && !request.isHttpError;
#endif
        }

        /// <summary> Get from server using UnitWebRequest </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="header">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Get(string uri, string[] header = null,
                                                      Action<float, float> progress = null)
        {
            var request = UnityWebRequest.Get(uri);

            return await request.SendWebRequest(header, progress);
        }

        /// <summary> Get from server using UnitWebRequest </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="header">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Get(Uri uri, string[] header = null,
                                                      Action<float, float> progress = null)
        {
            var request = UnityWebRequest.Get(uri);

            return await request.SendWebRequest(header, progress);
        }

        /// <summary>
        /// Get from server using UnitWebRequest
        /// </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="handler"> Handler for file downloading </param>
        /// <param name="header">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> GetFile(Uri uri, DownloadHandlerFile handler, string[] header = null,
                                                          Action<float, float> progress = null)
        {
            var request = UnityWebRequest.Get(uri);
            request.downloadHandler = handler;

            return await request.SendWebRequest(header, progress);
        }

        /// <summary> Post from server using UnitWebRequest without message </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="header">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Post(string uri, string[] header = null,
                                                       Action<float, float> progress = null)
        {
            var request = UnityWebRequest.PostWwwForm(uri, "");

            return await request.SendWebRequest(header, progress);
        }

        /// <summary> Post from server using UnitWebRequest without message </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="header">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Post(Uri uri, string[] header = null,
                                                       Action<float, float> progress = null)
        {
            var request = UnityWebRequest.PostWwwForm(uri, "");

            return await request.SendWebRequest(header, progress);
        }

        /// <summary> Get from server using UnitWebRequest with string message </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="message">String message</param>
        /// <param name="header">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Post(string uri, string message, string[] header = null,
                                                       Action<float, float> progress = null)
        {
            var request = UnityWebRequest.PostWwwForm(uri, message);

            return await request.SendWebRequest(header, progress);
        }

        /// <summary> Get from server using UnitWebRequest with WWWform message </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="message">WWWform message</param>
        /// <param name="header">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Post(string uri, WWWForm message, string[] header = null,
                                                       Action<float, float> progress = null)
        {
            var request = UnityWebRequest.Post(uri, message);

            return await request.SendWebRequest(header, progress);
        }

        /// <summary> Post to server using UnitWebRequest with multipart array </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="message">Multipart array</param>
        /// <param name="headers">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Post(string uri, List<IMultipartFormSection> message,
                                                       string[] headers = null, Action<float, float> progress = null)
        {
            var request = UnityWebRequest.Post(uri, message);

            return await request.SendWebRequest(headers, progress);
        }

        /// <summary> Create a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data. </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory.</param>
        /// <param name="headers">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Image(string uri, bool nonReadable = true, string[] headers = null,
                                                        Action<float, float> progress = null)
        {
            var request = UnityWebRequestTexture.GetTexture(uri, nonReadable);

            return await request.SendWebRequest(headers, progress);
        }

        /// <summary> Create a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data. </summary>
        /// <param name="uri">Full uri of request</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory.</param>
        /// <param name="headers">Headers add to request</param>
        /// <param name="progress"> Progress event handler. </param>
        /// <returns>Task to finished UnityWebRequest</returns>
        public static async Task<UnityWebRequest> Image(Uri uri, bool nonReadable = true, string[] headers = null,
                                                        Action<float, float> progress = null)
        {
            var request = UnityWebRequestTexture.GetTexture(uri, nonReadable);

            return await request.SendWebRequest(headers, progress);
        }

        private static async Task<UnityWebRequest> SendWebRequest(this UnityWebRequest request, string[] header,
                                                                  Action<float, float> progress)
        {
            if (request == null) return null;

            if (header != null)
            {
                for (int i = 0; i < header.Length / 2; i += 2)
                {
                    request.SetRequestHeader(header[i], header[i + 1]);
                }
            }

            // Send the request and wait for a response
            if (progress == null) await request.SendWebRequest();
            else
            {
                var send = request.SendWebRequest();

                while (!send.isDone)
                {
                    progress.Invoke(request.uploadProgress, request.downloadProgress);
                    await Task.Yield();
                }
            }

            return request;
        }

        private static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}