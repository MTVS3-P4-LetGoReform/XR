using System;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class RankingTrigger : MonoBehaviour
{
    public Canvas rankingCanvas;
    public Button[] closeButton;

    private bool _canInteract = true;
    private bool _isTrigger = false;
    
    private NetworkObject _playerNetworkObject;


    private void Start()
    {
        foreach (var button in closeButton)
        {
            button.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    //플레이어가 영역내 접근 시
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

    //플레이어가 영역에서 벗어났을 때
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
        if (!_isTrigger || _playerNetworkObject == null ||
            _playerNetworkObject.InputAuthority != RunnerManager.Instance.runner.LocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _canInteract = false;
            OpenNpcCanvas();
        }
    }

    private void OpenNpcCanvas()
    {
        rankingCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseNpcCanvas()
    {
        rankingCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        _canInteract = true;
    }
    

    // 창닫기 버튼에서 호출할 메서드
    private void OnCloseButtonClicked()
    {
        CloseNpcCanvas();
    }
}
    
