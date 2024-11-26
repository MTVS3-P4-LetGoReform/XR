using System;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTriggerHandler : MonoBehaviour
{
    public Canvas npcCanvas;
    public GameObject npcChoose;
    public Transform viewPoint;
    public Button[] closeButton;
    
    private bool _canInteract = true;
    private bool _isTrigger = false;
    private NetworkObject _playerNetworkObject;
    private Camera _playerCamera;

    private const float TransitionSpeed = 2f; // 카메라 이동 속도
    private bool _isCameraTransitioning = false;
    

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

            if (_playerCamera == null)
            {
                _playerCamera = _playerNetworkObject.GetComponentInChildren<Camera>();
            }
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

        // 카메라 이동 처리
        if (_isCameraTransitioning && _playerCamera != null)
        {
            SmoothCameraTransition(viewPoint.position, viewPoint.rotation, () => _isCameraTransitioning = false);
        }
    }

    private void OpenNpcCanvas()
    {
        npcCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerCameraLock();
    }

    private void CloseNpcCanvas()
    {
        npcCanvas.enabled = false;
        npcChoose.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        ReturnPlayerCameraToOriginal();
        _canInteract = true;
    }

    private void PlayerCameraLock()
    {
        _isCameraTransitioning = true; // 카메라 전환 시작
        PlayerInput.PlayerLockCamera(true); 
    }

    private void ReturnPlayerCameraToOriginal()
    {
        PlayerInput.PlayerLockCamera(false); 
    }

    private void SmoothCameraTransition(Vector3 targetPosition, Quaternion targetRotation, Action onComplete)
    {
        // 카메라 위치를 부드럽게 전환
        _playerCamera.transform.position = Vector3.Lerp(
            _playerCamera.transform.position,
            targetPosition,
            Time.deltaTime * TransitionSpeed
        );

        // 카메라 회전을 부드럽게 전환
        _playerCamera.transform.rotation = Quaternion.Lerp(
            _playerCamera.transform.rotation,
            targetRotation,
            Time.deltaTime * TransitionSpeed
        );

        // 전환 완료 여부 확인
        if (Vector3.Distance(_playerCamera.transform.position, targetPosition) < 0.01f &&
            Quaternion.Angle(_playerCamera.transform.rotation, targetRotation) < 0.1f)
        {
            onComplete.Invoke(); // 전환 완료 시 콜백 호출
        }
    }

    // 창닫기 버튼에서 호출할 메서드
    public void OnCloseButtonClicked()
    {
        CloseNpcCanvas();
    }
}
