#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System.Threading.Tasks;
using com.in3d.sdk.import;
using UnityEngine;

namespace com.in3d.sdk.loaders
{
    /// <summary> Interface for classes that loads avatars to scene. </summary>
    /// <remarks> It isn't actual interface because of interfaces can't be linked through unity editor. </remarks>
    public abstract class ModelLoaderBase : MonoBehaviour
    {
        [Tooltip("Info of avatar that is loaded to scene.")]
        [SerializeField] protected AvatarInfo Avatar;

#pragma warning disable CS1998
        private async void Start()
#pragma warning restore CS1998
        {
#pragma warning disable CS4014
            if (Avatar != null) LoadAvatarAsync(Avatar);
#pragma warning restore CS4014
        }

        /// <summary> Loads avatar to scene. </summary>
        public abstract Task<bool> LoadAvatarAsync(AvatarInfo avatar);
    }
}

#endif