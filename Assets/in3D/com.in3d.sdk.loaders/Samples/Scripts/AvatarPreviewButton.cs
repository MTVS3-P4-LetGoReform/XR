#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System.Threading.Tasks;
using com.in3d.sdk.import;
using UnityEngine;
using UnityEngine.UI;

namespace com.in3d.sdk.loaders.Samples
{
    /// <summary> UI of preview button </summary>
    public class AvatarPreviewButton : MonoBehaviour
    {
        [SerializeField] private Button LoadButton;
        [SerializeField] private Button SaveIdToEditorButton;

        public async Task Setup(ModelLoaderBase loaderBase, AvatarInfo avatar)
        {
            var image = LoadButton.GetComponentInChildren<RawImage>();
            image.texture = await avatar[ModelFormat.Glb].LoadPreviewAsync();
            LoadButton.onClick.AddListener(() => loaderBase.LoadAvatarAsync(avatar));
            SaveIdToEditorButton.onClick.AddListener(() => avatar.SaveToEditor());
        }
    }
}

#endif