#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System;
using System.Threading.Tasks;
using com.in3d.sdk.import;
using GLTFast;
using UnityEngine;

namespace com.in3d.sdk.loaders
{
    /// <summary> Runtime loader of Glb avatar models. </summary>
    [RequireComponent(typeof(GltfAsset))]
    [RequireComponent(typeof(Animator))]
    public class GlbLoader : ModelLoaderBase
    {
        [Tooltip("Animation controller that will be automatically set to all loaded models.")]
        [SerializeField] public RuntimeAnimatorController Controller;

        /// <inheritdoc />
        public override async Task<bool> LoadAvatarAsync(AvatarInfo avatar)
        {
            Avatar = avatar;
            ClearModel();
            var asset = GetComponent<GltfAsset>();
            Uri url;

            if (avatar[ModelFormat.Glb].ModelUrl != null)
            {
                url = avatar[ModelFormat.Glb].ModelUrl;
            }
            // else if (avatar[ModelFormat.Vto].ModelUrl != null)
            // {
            //     url = avatar[ModelFormat.Vto].ModelUrl;
            // }
            else return false;

            var success = await asset.Load(url.ToString());
            if (!success) return false;
            PrepareModel(transform);
            return true;
        }

        private void ClearModel()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void PrepareModel(Transform model)
        {
            var root = model.transform.Find("Scene"); // TODO make naming independent
            if (!root) return;
            root = root.Find("Armature"); // TODO make naming independent
            if (!root) return;
            Destroy(root.GetComponent<Animation>()); // Not always

            var animator = root.gameObject.GetComponent<Animator>();
            if (!animator) animator = root.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = Controller;
            animator.applyRootMotion = true;

            if (!root.gameObject.GetComponent<HumanoidAvatarBuilder>())
                root.gameObject.AddComponent<HumanoidAvatarBuilder>();

            // not necessary, works fine without
            var skin = root.GetComponentInChildren<SkinnedMeshRenderer>();
            var hips = root.Find("mixamorig:Hips");
            skin.rootBone = hips;
        }
    }
}

#endif