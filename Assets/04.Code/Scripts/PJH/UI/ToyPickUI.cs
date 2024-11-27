using System;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ToyPickUI : MonoBehaviour
{
    public GameObject pressE;
    public GameObject statue;
    public Button[] closeButton;
    
    private bool _canInteract = true;
    private bool _isTrigger = false;
    private NetworkObject _playerNetworkObject;

    private void Start()
    {
        pressE.SetActive(false);
        foreach (var button in closeButton)
        {
            button.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _canInteract)
        {
            Debug.Log(_isTrigger);

            var ntObject = other.GetComponent<NetworkObject>();
            if (ntObject.InputAuthority != RunnerManager.Instance.runner.LocalPlayer)
                return;

            pressE.SetActive(true);
            
            _isTrigger = true;
            _playerNetworkObject = ntObject;
            
        }
    }

    //플레이어가 영역에서 벗어났을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var ntObject = other.GetComponent<NetworkObject>();
            if (ntObject.InputAuthority != RunnerManager.Instance.runner.LocalPlayer)
                return;

            pressE.SetActive(false);
            
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
            OpenNpcCanvas();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _canInteract = false;
            CloseNpcCanvas();
        }
        
    }

    private void OpenNpcCanvas()
    {
        statue.SetActive(true);
        pressE.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        PlayerInput.PlayerLockCamera(true);
    }

    private void CloseNpcCanvas()
    {
        statue.SetActive(false);
        pressE.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInput.PlayerLockCamera(false); 
        _canInteract = true;
    }
    

    // 창닫기 버튼에서 호출할 메서드
    private void OnCloseButtonClicked()
    {
        _canInteract = false;
        CloseNpcCanvas();
    }
}
