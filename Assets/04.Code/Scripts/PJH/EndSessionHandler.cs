using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSessionHandler : MonoBehaviour
{
    public Button endButton;

    private void Start()
    {
        endButton.onClick.AddListener(EndSession);
    }

    private async void EndSession()
    {
        await NetworkManager.Instance.ShutdownRunner();
        await NetworkManager.Instance.JoinPublicSession();
        SceneManager.LoadScene(0);
    }
}