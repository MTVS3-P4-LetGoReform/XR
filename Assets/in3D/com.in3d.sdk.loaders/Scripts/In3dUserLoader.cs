#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System;
using System.Linq;
using System.Threading.Tasks;
using com.in3d.sdk.import;
using com.in3d.sdk.import.Credentials;
using com.in3d.sdk.import.Users;
using UnityEngine;

namespace com.in3d.sdk.loaders
{
    /// <summary> Class for loading user and its avatars. </summary>
    public class In3dUserLoader : MonoBehaviour
    {
        [Tooltip("Credentials of loaded user. It will be used on Start if set.")]
        [SerializeField] private CredentialsBase Credentials;

        [Tooltip("Avatars owned by user.")]
        [SerializeField] private AvatarInfo[] Avatars;

        private IUser _user;

        /// <summary> Loader for avatars models. </summary>
        public ModelLoaderBase Target;

        /// <summary> This event will be raised when fetching avatars list was started. </summary>
        /// <remarks> First param is user ID, second is amount of user's avatars. </remarks>
        public event Action<string, int> OnAvatarsListUpdateStarted;
        
        /// <summary> This event will be raised when fetching avatars list was failed. </summary>
        public event Action OnAvatarsListUpdateFailed;

        /// <summary> This event will be raised when fetching avatars list was completed. </summary>
        public event Action<AvatarInfo[]> OnAvatarsListUpdated;
        
        /// <summary> This event will be raised when one avatar was fetched. </summary>
        public event Action<AvatarInfo> OnAvatarLoaded;

        /// <summary> Changes user. </summary>
        // ReSharper disable once ParameterHidesMember
        public async Task SetCredentials(CredentialsBase credentials)
        {
            Credentials = credentials;
            await GetAvatars();
        }

        private void Start()
        {
#pragma warning disable CS4014
            if (Credentials != null) GetAvatars();
#pragma warning restore CS4014
        }

        private async Task GetAvatars()
        {
            try
            {
                _user = await Credentials.LoginAsync();
            }
            catch
            {
                Debug.LogError("Could not login user with given credentials");
                OnAvatarsListUpdateFailed?.Invoke();
                throw;
            }

            _user.OnAvatarsUpdateStarted += amount => OnAvatarsListUpdateStarted?.Invoke(_user.Id, amount);
            _user.OnAvatarLoaded += info => OnAvatarLoaded?.Invoke(info);
            
            await _user.UpdateAvatarsListAsync();
            Avatars = _user.Avatars.ToArray();
            OnAvatarsListUpdated?.Invoke(Avatars);
        }
    }
}

#endif