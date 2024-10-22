using Fusion;
using UnityEngine;

public class LobbyTriggerHandler : MonoBehaviour
{
    public Canvas sessionCanvas;
    private bool _canInteract = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _canInteract && Input.GetKeyDown(KeyCode.E))
        {
            _canInteract = false;
            ToggleSessionCanvas();
            _canInteract = true;
        }
    }

    private async void ToggleSessionCanvas()
    {
        sessionCanvas.enabled = !sessionCanvas.enabled;
        if (sessionCanvas.enabled)
        {
            await NetworkManager.Instance.ShutdownRunner();
            await NetworkManager.Instance.JoinLobby();
        }
        else
        {
            await NetworkManager.Instance.ShutdownRunner();
            await NetworkManager.Instance.JoinPublicSession();
        }
    }
}
