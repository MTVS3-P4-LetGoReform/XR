#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System.IO;
using System.Threading.Tasks;
using com.in3d.sdk.import.Users;
using UnityEditor;
using UnityEngine;

namespace com.in3d.sdk.import.Credentials
{
    /// <summary> Interface for all types of In3d user credentials. </summary>
    /// <remarks> It isn't actual interface because of interfaces can't be linked through unity editor. </remarks>
    public abstract class CredentialsBase : ScriptableObject
    {
        /// <summary> Server where user will be logged in. </summary>
        [SerializeField] protected ServerSettings ServerSettings;

        /// <summary> Unique identification of this credentials. </summary>
        public abstract string Id { get; }
        
        /// <summary> Authenticate user. </summary>
        /// <returns> User object. </returns>
        public abstract Task<IUser> LoginAsync();
        
        /// <summary> Saves this object as asset. </summary>
        public void SaveToEditor(string folderPath = "Assets/")
        {
            AssetDatabase.CreateAsset(this, Path.Combine(folderPath, $"Credentials {Id}.asset"));
        }
    }
}

#endif