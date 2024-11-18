using UnityEngine;
using System;

public class FriendListManager : MonoBehaviour
{
    [SerializeField] private Canvas friendListCanvas; // Inspector에서 캔버스를 할당
    
    private void Start()
    {
        if (friendListCanvas == null)
        {
            friendListCanvas = GetComponent<Canvas>();
            if (friendListCanvas == null)
            {
                Debug.LogError("Canvas 컴포넌트를 찾을 수 없습니다.");
                return;
            }
        }

        friendListCanvas.enabled = false; // 초기 상태 설정
        PlayerInput.OnMessenger += OpenMessenger;
    }

    private void OnDestroy()
    {
        PlayerInput.OnMessenger -= OpenMessenger; // 이벤트 구독 해제
    }

    private void OpenMessenger(bool open)
    {
        if (friendListCanvas != null)
        {
            friendListCanvas.enabled = open;
        }
        else
        {
            Debug.LogWarning("Friend List Canvas가 null입니다. 상태를 변경할 수 없습니다.");
        }
    }
}