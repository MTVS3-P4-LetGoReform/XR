#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using com.in3d.sdk.import.Web;
using UnityEngine;

namespace com.in3d.sdk.import.Users
{
    /// <summary> In3d user that uses access key from mobile app for authentication. </summary>
    public class AccessKeyUser : IUser
    {
        private readonly string _accessKey;
        private readonly ServerSettings _serverSettings;
        private readonly string[] _authorizationHeader;

        /// <summary> Constructor. </summary>
        /// <param name="accessKey"> Access key copied from mobile app. </param>
        /// <param name="serverSettings"> Server. </param>
        public AccessKeyUser(string accessKey, ServerSettings serverSettings)
        {
            _accessKey = accessKey;
            _serverSettings = serverSettings;
            _authorizationHeader = new[] { "Authorization", $"Bearer {accessKey}" };
        }

        /// <inheritdoc />
        public string Id => _accessKey;

        /// <inheritdoc />
        public virtual async Task UpdateAvatarsListAsync()
        {
            var uri = new Uri(_serverSettings.Url, "/v2/user_avatars/list_with_token");
            string[] avatarIds;
            using (var req = await Requests.Post(uri, _authorizationHeader))
            {
                if (!req.Success()) throw new HttpRequestException(req.error);

                var json = req.downloadHandler.text.FormatJsonText(typeof(JsonArray<string>));
                avatarIds = JsonUtility.FromJson<JsonArray<string>>(json).list;
            }

            _avatars.Clear();
            OnAvatarsUpdateStarted?.Invoke(avatarIds.Length);
            foreach (var avatarId in avatarIds)
            {
                if (_avatars.ContainsKey(avatarId))
                {
                    await UpdateAvatarAsync(_avatars[avatarId]);
                }
                else
                {
                    _avatars[avatarId] = await LoadAvatarByIdAsync(Guid.Parse(avatarId));
                }

                OnAvatarLoaded?.Invoke(_avatars[avatarId]);
            }

            OnAvatarsUpdated?.Invoke();
        }

        /// <inheritdoc />
        public event Action<int> OnAvatarsUpdateStarted;

        /// <inheritdoc />
        public event Action<AvatarInfo> OnAvatarLoaded;

        /// <inheritdoc />
        public event Action OnAvatarsUpdated;

        /// <inheritdoc />
        public IReadOnlyCollection<AvatarInfo> Avatars => _avatars.Values;

        /// <inheritdoc />
        public virtual async Task<AvatarInfo> LoadAvatarByIdAsync(Guid id)
        {
            var modelUrls = await GetUrls(id);
            return ScriptableObject.CreateInstance<AvatarInfo>().Init(id, modelUrls);
        }

        /// <inheritdoc />
        public virtual async Task UpdateAvatarAsync(AvatarInfo avatar)
        {
            var modelUrls = (await GetUrls(avatar.Id)).ToList();
            for (var i = 0; i < modelUrls.Count; i++)
            {
                avatar[(ModelFormat)i] = modelUrls[i];
            }
        }

        #region Equality

        protected bool Equals(AccessKeyUser other)
        {
            return _accessKey == other._accessKey;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AccessKeyUser)obj);
        }

        public override int GetHashCode()
        {
            return (_accessKey != null ? _accessKey.GetHashCode() : 0);
        }

        #endregion

        #region Private

        private Dictionary<string, AvatarInfo> _avatars = new Dictionary<string, AvatarInfo>();

        private class UrlResponse
        {
            public string url;
        }

        private async Task<IEnumerable<ModelUrls>> GetUrls(Guid avatarId)
        {
            var modelUrls = new List<Uri>();
            var previewUrls = new List<Uri>();
            foreach (var format in (ModelFormat[])Enum.GetValues(typeof(ModelFormat)))
            {
                try
                {
                    modelUrls.Add(await GetModelUrl(avatarId, format));
                }
                catch
                {
                    modelUrls.Add(null);
                }

                try
                {
                    previewUrls.Add(await GetPreviewUrl(avatarId, format));
                }
                catch
                {
                    previewUrls.Add(null);
                }
            }

            return modelUrls.Zip(previewUrls, (m, p) => new ModelUrls { ModelUrl = m, PreviewUrl = p });
        }

        private async Task<Uri> GetModelUrl(Guid avatarId, ModelFormat format)
        {
            var requestUrl = new Uri(_serverSettings.Url,
                                     $"/v2/user_avatars/model_with_token/{avatarId}?format={format.ToBackendString()}");
            return await GetUrl(requestUrl);
        }

        private async Task<Uri> GetPreviewUrl(Guid avatarId, ModelFormat format)
        {
            // TODO: use given format instead VTO when backend will be ready
            var requestUrl = new Uri(_serverSettings.Url,
                                     $"/v2/user_avatars/preview_with_token/{avatarId}?fmt=vto");
            return await GetUrl(requestUrl);
        }

        private async Task<Uri> GetUrl(Uri endpoint)
        {
            string json;
            using (var req = await Requests.Post(endpoint, _authorizationHeader))
            {
                if (!req.Success()) throw new HttpRequestException(req.error);
                json = req.downloadHandler.text;
            }

            if (string.IsNullOrEmpty(json)) throw new HttpRequestException("Empty response");

            var response = JsonUtility.FromJson<UrlResponse>(json.FormatJsonText(typeof(UrlResponse)));
            return new Uri(response.url);
        }

        #endregion
    }
}

#endif