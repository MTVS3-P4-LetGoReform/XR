using System;
using Fusion;
using UnityEngine;

public class LobbyTriggerHandler : MonoBehaviour
{
    public Canvas sessionCanvas;
    
    private bool _canInteract = true;
    private bool _isTrigger = false;
    private NetworkObject _playerNetworkObject;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _canInteract)
        {
            Debug.Log(_isTrigger);
            var ntObject = other.GetComponent<NetworkObject>();
            if (ntObject.InputAuthority != RunnerManager.Instance.runner.LocalPlayer)
                return;
            _isTrigger = true;
            _playerNetworkObject = ntObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var ntObject = other.GetComponent<NetworkObject>();
            if (ntObject.InputAuthority != RunnerManager.Instance.runner.LocalPlayer)
                return;
            _isTrigger = false;
        }
    }
    
    private void Update()
    {
        if (!_isTrigger || _playerNetworkObject == null || _playerNetworkObject.InputAuthority != RunnerManager.Instance.runner.LocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _canInteract = false;
            JoinLobbyTrigger();
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
}
