#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System;
using System.Threading.Tasks;
using com.in3d.sdk.import.Users;
using com.in3d.sdk.import.Web;
using UnityEngine;

namespace com.in3d.sdk.import.Credentials
{
    /// <summary> Credentials for users that use access key from mobile in3d application. </summary>
    [CreateAssetMenu(fileName = "AccessKeyCredentials", menuName = "in3d/Access Key Credentials", order = 0)]
    public class AccessKeyCredentials : CredentialsBase
    {
        [Tooltip("Access key from In3d mobile application.")] [SerializeField]
        private string accessKey;

        /// <summary> Access key from In3d mobile application. </summary>
        public string AccessKey => accessKey;

        /// <inheritdoc />
        public override string Id => accessKey;

        /// <inheritdoc />
        public override async Task<IUser> LoginAsync()
        {
            if (!(await ValidateAccessKeyAsync()))
            {
                throw new InvalidCredentialsException();
            }

            return new AccessKeyUser(accessKey, ServerSettings);
        }

        /// <summary> Factory method for creating new access key credentials. </summary>
        public static AccessKeyCredentials Create(string accessKey, ServerSettings serverSettings)
        {
            var result = CreateInstance<AccessKeyCredentials>();
            result.ServerSettings = serverSettings;
            result.accessKey = accessKey;
            return result;
        }

        private async Task<bool> ValidateAccessKeyAsync()
        {
            var requestUrl = new Uri(ServerSettings.Url, $"v2/user_avatars/list_with_token");

            using (var req = await Requests.Post(requestUrl.ToString(),
                                                 new[] { "Authorization", $"Bearer {accessKey}" }))
            {
                return req.Success();
            }
        }
        
    }
}

#endif