using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSessionHandler : MonoBehaviour
{
    public Button endButton;

    private const string PublicSession = "FusionTest 1";
    private void Start()
    {
        endButton.onClick.AddListener(EndSession);
    }

    private async void EndSession()
    {
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.JoinPublicSession();
        SceneManager.LoadScene(PublicSession);
    }
}