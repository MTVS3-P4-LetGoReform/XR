using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace com.in3d.sdk.import
{
    /// <summary> Scriptable object for storing server settings, such as server's url etc. </summary>
    [CreateAssetMenu(fileName = "ServerSettings", menuName = "in3d/Server Settings", order = 0)]
    public class ServerSettings : ScriptableObject
    {
        [Tooltip("Server's url")] [SerializeField]
        private string url;

        [Tooltip("Server's vendor")] [SerializeField]
        private string vendor = "in3d";

        /// <summary> Server's url </summary>
        public Uri Url => new Uri(url);

        /// <summary> Server's vendor </summary>
        public string Vendor => vendor;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public ServerSettings Init(Uri uri, string vendor = "in3d")
        {
            url = uri.ToString();
            this.vendor = vendor;
            return this;
        }

        public static ServerSettings Default => CreateInstance<ServerSettings>().Init(new Uri("https://app.gsize.io"));
    }
}