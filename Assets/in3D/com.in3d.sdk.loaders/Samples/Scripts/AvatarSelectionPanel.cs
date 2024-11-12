#if IN3D_SDK_DEPENDENCIES_RESOLVED

using System.Collections;
using System.Threading.Tasks;
using com.in3d.sdk.import;
using UnityEngine;
using UnityEngine.UI;

namespace com.in3d.sdk.loaders.Samples
{
    /// <summary> UI for selecting avatars by clicking on their previews. </summary>
    public class AvatarSelectionPanel : MonoBehaviour
    {
        [SerializeField] private In3dUserLoader UserLoader;
        [SerializeField] private AvatarPreviewButton PreviewButtonPrefab;
        [SerializeField] private RectTransform CanvasTarget;
        [SerializeField] private Text LoadingBar;
        [SerializeField] private GameObject ErrorTextLabel;

        private int _amountOfAvatars, _currentAmount;
        
        private void Start()
        {
            UserLoader.OnAvatarsListUpdateStarted += (_, amount) => StartUpdate(amount);
            UserLoader.OnAvatarsListUpdateFailed += () =>
            {
                StartCoroutine(ShowLoginErrorCoroutine());
                LoadingBar.gameObject.SetActive(false);
            };
            UserLoader.OnAvatarLoaded += async info => await AddAvatarPreview(info);
            UserLoader.OnAvatarsListUpdated += _ => LoadingBar.gameObject.SetActive(false);;
        }

        private IEnumerator ShowLoginErrorCoroutine()
        {
            ErrorTextLabel.SetActive(true);
            yield return new WaitForSeconds(10);
            ErrorTextLabel.SetActive(false);
        }

        private void StartUpdate(int amount)
        {
            _currentAmount = 0;
            _amountOfAvatars = amount;
            LoadingBar.gameObject.SetActive(true);
            SetMessage();
            ClearPreviews();
        }

        private void SetMessage()
        {
            LoadingBar.text = $"Please wait while avatars are loading ({_currentAmount}/{_amountOfAvatars})";
        }

        private async Task AddAvatarPreview(AvatarInfo avatar)
        {
            var button = Instantiate(PreviewButtonPrefab, CanvasTarget);
            await button.Setup(UserLoader.Target, avatar);
            _currentAmount++;
            SetMessage();
        }

        private void ClearPreviews()
        {
            for (var i = 0; i < CanvasTarget.childCount; i++)
            {
                Destroy(CanvasTarget.GetChild(i).gameObject);
            }
        }
    }
}

#endif