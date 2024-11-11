#if IN3D_SDK_DEPENDENCIES_RESOLVED

using com.in3d.sdk.import;
using com.in3d.sdk.import.Credentials;
using UnityEngine;
using UnityEngine.UI;

namespace com.in3d.sdk.loaders.Samples
{
    /// <summary> UI that creates AccessKeyCredentials </summary>
    public class CredentialsCreator : MonoBehaviour
    {
        [SerializeField] private Button LoginButton;
        [SerializeField] private InputField AccessKeyInputField;
        [SerializeField] private In3dUserLoader UserLoader;
        [SerializeField] private ServerSettings ServerSettings;

        public void Start()
        {
            LoginButton.onClick.AddListener(() =>
            {
                LoginButton.interactable = false;
                var creds = AccessKeyCredentials.Create(AccessKeyInputField.text, ServerSettings);
#pragma warning disable CS4014
                UserLoader.SetCredentials(creds);
#pragma warning restore CS4014
            });
            UserLoader.OnAvatarsListUpdateStarted += (key, _) => AccessKeyInputField.text = key;
            UserLoader.OnAvatarsListUpdated += _ => LoginButton.interactable = true;
            UserLoader.OnAvatarsListUpdateFailed += () => LoginButton.interactable = true;
        }
    }
}

#endif