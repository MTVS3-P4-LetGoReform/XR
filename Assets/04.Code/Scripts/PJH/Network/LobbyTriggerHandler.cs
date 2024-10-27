using System;
using Fusion;
using UnityEngine;

public class LobbyTriggerHandler : MonoBehaviour
{
    public Canvas sessionCanvas;
    private bool _canInteract = true;

    private void OnTriggerStay(Collider other)
    {
        /*if (other.CompareTag("Player") && _canInteract && Input.GetKeyDown(KeyCode.E))
        {
            _canInteract = false;
            ToggleSessionCanvas();
            _canInteract = true;
        }*/
    }

    private void Update()
    {
        if (_canInteract && Input.GetKeyDown(KeyCode.E))
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
            Cursor.lockState = CursorLockMode.None;
            await RunnerManager.Instance.ShutdownRunner();
            await RunnerManager.Instance.JoinLobby();
           
        }
        else
        {
            await RunnerManager.Instance.ShutdownRunner();
            await RunnerManager.Instance.JoinPublicSession();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
