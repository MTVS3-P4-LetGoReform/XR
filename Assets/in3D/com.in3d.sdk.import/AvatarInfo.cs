#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using com.in3d.sdk.import.Web;
using UnityEditor;
using UnityEngine;

namespace com.in3d.sdk.import
{
    /// <summary>
    /// Scriptable object for storing avatar's info such as it's ID, urls for downloading models, ect.
    /// </summary>
    public class AvatarInfo : ScriptableObject
    {
        [Tooltip("ID for this avatar")] [SerializeField]
        private string ID;

        /// <summary> ID for this avatar </summary>
        public virtual Guid Id => Guid.Parse(ID);

        [Tooltip("Urls for downloading previews of this avatar. " +
                 "One for each format of avatar's model. " +
                 "Same order as in ModelFormat enumeration.")]
        [SerializeField]
        private List<string> PreviewUrls = new List<string>();

        [Tooltip("Urls for downloading models of this avatar. " +
                 "One for each format of avatar's model. " +
                 "Same order as in ModelFormat enumeration.")]
        [SerializeField]
        private List<string> ModelUrls = new List<string>();

        /// <summary> Indexer that allows to get model and preview url by format. </summary>
        /// <param name="format"> Requested format of model </param>
        public virtual ModelUrls this[ModelFormat format]
        {
            get => new ModelUrls
            {
                PreviewUrl = PreviewUrls[(int)format] != null ? new Uri(PreviewUrls[(int)format]) : null,
                ModelUrl = ModelUrls[(int)format] != null ? new Uri(ModelUrls[(int)format]) : null,
            };
            set
            {
                PreviewUrls[(int)format] = value.PreviewUrl?.ToString();
                ModelUrls[(int)format] = value.ModelUrl?.ToString();
            }
        }

        /// <summary> Initializes this object with values. </summary>
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        [SuppressMessage("ReSharper", "LocalVariableHidesMember")]
        public virtual AvatarInfo Init(Guid id, IEnumerable<ModelUrls> urls)
        {
            ID = id.ToString();
            foreach (var modelUrls in urls)
            {
                ModelUrls.Add(modelUrls.ModelUrl?.ToString());
                PreviewUrls.Add(modelUrls.PreviewUrl?.ToString());
            }

            return this;
        }

        /// <summary> Saves this object to asset </summary>
        public virtual void SaveToEditor(string folderPath = "Assets/")
        {
            AssetDatabase.CreateAsset(this, Path.Combine(folderPath, $"Avatar {ID}.asset"));
        }

        /// <summary> Saves fbx model of this avatar to asset </summary>
        public virtual async Task SaveFbxToEditorAsync(string folderPath = "Assets/")
        {
            var url = this[ModelFormat.Fbx].ModelUrl;
            var request = await Requests.Get(url);
            if (!request.Success())
            {
                Debug.LogError($"Can't download fbx: {request.error}");
                return;
            }

            try
            {
                var filename = $"AvatarModel {Id}.fbx";
                File.WriteAllBytes(Path.Combine(Application.dataPath, filename), request.downloadHandler.data);
                AssetDatabase.ImportAsset(Path.Combine(folderPath, filename));
                var importer = (ModelImporter)AssetImporter.GetAtPath(Path.Combine(folderPath, filename));
                importer.ExtractTextures(folderPath);
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary> Saves glb model of this avatar to asset </summary>
        /// <remarks>
        /// You will not be able to open use this asset if you don't have some glb importer installed to unity.
        /// We recommend to use this one: https://github.com/atteneder/glTFast
        /// </remarks>
        public virtual async Task SaveGlbToEditorAsync(string folderPath = "Assets/")
        {
            var url = this[ModelFormat.Glb].ModelUrl;
            var request = await Requests.Get(url);
            if (!request.Success())
            {
                Debug.LogError($"Can't download glb: {request.error}");
                return;
            }

            try
            {
                var filename = $"AvatarModel {Id}.glb";
                File.WriteAllBytes(Path.Combine(Application.dataPath, filename), request.downloadHandler.data);
                AssetDatabase.ImportAsset(Path.Combine(folderPath, filename));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #region Equality

        protected bool Equals(AvatarInfo other)
        {
            return base.Equals(other) && ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AvatarInfo)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (ID != null ? ID.GetHashCode() : 0);
            }
        }

        #endregion
    }
}

#endif