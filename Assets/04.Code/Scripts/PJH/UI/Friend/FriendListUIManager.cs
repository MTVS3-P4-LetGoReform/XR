using UnityEngine;
using System;
using System.Collections.Generic;

public class FriendListUIManager : MonoBehaviour
{
    public Transform friendListParent;  // 친구 목록을 보여줄 부모 오브젝트 (스크롤뷰의 Content)
    public GameObject friendItemPrefab; // 친구 항목 프리팹
    
    private string _currentUserId; // 로그인한 유저의 ID
    
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
        FriendRequestUIManager.RefreshList += RefreshList;
        PlayerInput.OnMessenger += OpenMessenger;
        UserData.ChangeName += OnChangedUserId;
        OnChangedUserId();
        RefreshList();
    }

    private void OnChangedUserId()
    {
        _currentUserId = UserData.Instance.UserId;
    }
    
    private void OnDestroy()
    {
        PlayerInput.OnMessenger -= OpenMessenger; // 이벤트 구독 해제
    }

    private void OpenMessenger(bool isActive)
    {
        if (friendListCanvas != null)
        {
            friendListCanvas.enabled = isActive;
        }
        else
        {
            Debug.LogWarning("Friend List Canvas가 null입니다. 상태를 변경할 수 없습니다.");
        }
    }

    /// <summary>
    /// 친구 목록을 새로고침합니다.
    /// </summary>
    public void RefreshList()
    {
        FriendRequestManager.GetFriends(_currentUserId,
            onSuccess: friendIds =>
            {
                // 기존 UI 정리
                foreach (Transform child in friendListParent)
                {
                    Destroy(child.gameObject);
                }

                if (friendIds.Count > 0)
                {
                    List<string> friendPaths = new List<string>();
                    foreach (var friendId in friendIds)
                    {
                        friendPaths.Add($"users/{friendId}");
                        Debug.Log($"FriendListUIManager - userEntry.Value : users/{friendId}");
                    }

                    // 친구 정보를 한 번에 로드
                    RealtimeDatabase.ReadMultipleData<User>(friendPaths,
                        onSuccess: users =>
                        {
                            foreach (var userEntry in users)
                            {
                                GameObject friendItem = Instantiate(friendItemPrefab, friendListParent);
                                FriendItem itemScript = friendItem.GetComponent<FriendItem>();
                                Debug.Log("FriendListUIManager - userEntry.Value : "+ userEntry.Value);
                                itemScript.friendId = userEntry.Key;
                                itemScript.SetFriendData(userEntry.Value); // UI에 친구 정보 설정
                            }
                        },
                        onFailure: exception =>
                        {
                            Debug.LogError($"친구 정보 일괄 로드 실패: {exception.Message}");
                        });
                }
            },
            onFailure: exception => Debug.LogError($"친구 목록 로드 실패: {exception.Message}"));
    }
}