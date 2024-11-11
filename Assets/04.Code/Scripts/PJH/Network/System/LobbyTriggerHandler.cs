using System;
using Fusion;
using UnityEngine;

public class LobbyTriggerHandler : MonoBehaviour
{
    public Canvas sessionCanvas;
    private bool _canInteract = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _canInteract && Input.GetKeyDown(KeyCode.E))
        {
            _canInteract = false;
            JoinLobbyTrigger();
        }
    }

    private void Update()
    {
        if (sessionCanvas.enabled &&_canInteract && Input.GetKeyDown(KeyCode.Escape))
        {
            _canInteract = false;
            JoinPublicSessionTrigger();
            _canInteract = true;
        }
    }

    private async void JoinLobbyTrigger()
    {
        sessionCanvas.enabled = true;
        Debug.Log("Interact");
        Cursor.lockState = CursorLockMode.None;
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.JoinLobby();
        _canInteract = true;
    }

    private async void JoinPublicSessionTrigger()
    {
        sessionCanvas.enabled = false;
        Debug.Log("Interact");
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.JoinPublicSession();
        Cursor.lockState = CursorLockMode.Locked;
    }
}
