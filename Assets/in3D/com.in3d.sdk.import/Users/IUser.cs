#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.in3d.sdk.import.Users
{
    /// <summary> Interface for classes those allows to load avatars for user. </summary>
    public interface IUser
    {
        /// <summary> Id of user. </summary>
        string Id { get; }

        /// <summary> Fetches list of avatars from server. </summary>
        Task UpdateAvatarsListAsync();

        /// <summary>
        /// This event will be raised when fetching avatars from server was started.
        /// Parameter is amount of avatars to be loaded.
        /// </summary>
        event Action<int> OnAvatarsUpdateStarted;

        /// <summary> This event will be raised when one avatar was loaded. </summary>
        event Action<AvatarInfo> OnAvatarLoaded;

        /// <summary> This event will be raised when all avatars was loaded. </summary>
        event Action OnAvatarsUpdated;

        /// <summary> Container of all avatars of this user by their Ids. </summary>
        IReadOnlyCollection<AvatarInfo> Avatars { get; }

        /// <summary> Loads avatar by its id. </summary>
        /// <param name="id"> Id of avatar. </param>
        Task<AvatarInfo> LoadAvatarByIdAsync(Guid id);

        /// <summary> Updates given avatar info object. </summary>
        Task UpdateAvatarAsync(AvatarInfo avatar);
    }
}

#endif