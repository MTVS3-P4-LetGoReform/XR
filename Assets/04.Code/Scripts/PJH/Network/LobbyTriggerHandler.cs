using System;
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
            Debug.Log("이게 왜 두번??");
            JoinLobbyTrigger();
        }
    }

    private void Update()
    {
        if (_canInteract && Input.GetKeyDown(KeyCode.Escape))
        {
            _canInteract = false;
            JoinPublicSessionTrigger();
            _canInteract = true;
        }
    }

    private async void ToggleSessionCanvas()
    {
        sessionCanvas.enabled = !sessionCanvas.enabled;
        if (sessionCanvas.enabled)
        {
            
           
        }
        else
        {
            
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
