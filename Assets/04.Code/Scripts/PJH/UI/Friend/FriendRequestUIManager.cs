using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;

public class FriendRequestUIManager : MonoBehaviourSingleton<FriendRequestUIManager>
{
    public static event Action RefreshList;
    
    [Header("UI References")]
    public Transform friendRequestListParent;        // 친구 요청 목록을 표시할 부모 Transform
    public GameObject friendRequestItemPrefab;       // 친구 요청 항목 프리팹
    public TMP_InputField usernameInputField;       // 친구 요청을 보낼 사용자의 닉네임 입력 필드
    public Button sendRequestButton;                 // 친구 요청 전송 버튼
    
    private string _currentUserId;                   // 현재 로그인한 사용자의 ID
    
    private void Start()
    {
        if (UserData.Instance.UserId == null)
        {
            Debug.LogError("아이디를 찾을 수 없습니다.");
            return;
        }
            
        _currentUserId = UserData.Instance.UserId;
    
        // 이벤트 구독 설정
        RefreshList += () => LoadInitialRequestsAsync().Forget();
        LoadInitialRequestsAsync().Forget();
        ListenForFriendRequests();

        // 친구 요청 전송 버튼 이벤트 설정
        sendRequestButton.onClick.AddListener(async () => 
        {
            string targetUsername = usernameInputField.text;
            if (string.IsNullOrEmpty(targetUsername))
            {
                Debug.LogWarning("닉네임을 입력하세요.");
                return;
            }

            await SendFriendRequestByUsernameAsync(targetUsername);
            Debug.Log($"친구 요청 전송: {targetUsername}");
        });
    }

    /// <summary>
    /// 초기 친구 요청 목록을 로드합니다.
    /// </summary>
    private async UniTask LoadInitialRequestsAsync()
    {
        try
        {
            var friendRequests = await RealtimeDatabase.ReadDataAsync<Dictionary<string, bool>>($"users/{_currentUserId}/friendRequests/incoming");
            await OnFriendRequestReceivedAsync(friendRequests);
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 요청 읽기 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 실시간 친구 요청 수신을 위한 리스너를 설정합니다.
    /// </summary>
    private void ListenForFriendRequests()
    {
        string path = $"users/{_currentUserId}/friendRequests/incoming";
        RealtimeDatabase.ListenForDataChanges<Dictionary<string, bool>>(
            path, 
            async (requests) => await OnFriendRequestReceivedAsync(requests), 
            OnFriendRequestError
        );
    }

    /// <summary>
    /// 친구 요청을 수신했을 때 UI를 업데이트합니다.
    /// </summary>
    /// <param name="friendRequests">수신된 친구 요청 목록</param>
    private async UniTask OnFriendRequestReceivedAsync(Dictionary<string, bool> friendRequests)
    {
        // 기존 UI 요소 제거
        foreach (Transform child in friendRequestListParent)
        {
            Destroy(child.gameObject);
        }

        if (friendRequests?.Count > 0)
        {
            // 요청자 정보 로드
            var requesterPaths = new List<string>();
            foreach (var request in friendRequests)
            {
                requesterPaths.Add($"users/{request.Key}");
            }

            try
            {
                var users = await RealtimeDatabase.ReadMultipleDataAsync<User>(requesterPaths);
                foreach (var userEntry in users)
                {
                    CreateFriendRequestItem(userEntry);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"친구 요청자 정보 일괄 로드 실패: {e.Message}");
            }
        }
    }

    /// <summary>
    /// 친구 요청 수신 중 오류 발생 시 처리합니다.
    /// </summary>
    private void OnFriendRequestError(Exception exception)
    {
        Debug.LogError($"친구 요청 수신 중 오류 발생: {exception.Message}");
    }

    /// <summary>
    /// 닉네임으로 친구 요청을 전송합니다.
    /// </summary>
    private async UniTask SendFriendRequestByUsernameAsync(string targetUsername)
    {
        try
        {
            await FriendRequestManager.SendFriendRequestByUsernameAsync(_currentUserId, targetUsername);
            Debug.Log($"닉네임 {targetUsername}로 친구 요청 전송 성공");
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 요청 전송 실패: {e.Message}");
        }
    }

    // UI 이벤트 래퍼 메서드들
    private void AcceptFriendRequest(string requesterId)
    {
        AcceptFriendRequestAsync(requesterId).Forget();
    }

    private void RejectFriendRequest(string requesterId)
    {
        RejectFriendRequestAsync(requesterId).Forget();
    }

    /// <summary>
    /// 친구 요청을 수락합니다.
    /// </summary>
    private async UniTask AcceptFriendRequestAsync(string requesterId)
    {
        try
        {
            await FriendRequestManager.AcceptFriendRequestAsync(_currentUserId, requesterId);
            Debug.Log("친구 요청 수락 성공");
            RefreshList?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 요청 수락 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 친구 요청을 거절합니다.
    /// </summary>
    private async UniTask RejectFriendRequestAsync(string requesterId)
    {
        try
        {
            await FriendRequestManager.RemoveFriendRequestAsync(_currentUserId, requesterId);
            Debug.Log("친구 요청 거절 성공");
            RefreshList?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 요청 거절 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 친구 요청 항목 UI를 생성합니다.
    /// </summary>
    private void CreateFriendRequestItem(KeyValuePair<string, User> userEntry)
    {
        GameObject friendRequestItem = Instantiate(friendRequestItemPrefab, friendRequestListParent);
        FriendRequestItem itemScript = friendRequestItem.GetComponent<FriendRequestItem>();
        
        string userId = userEntry.Key.Replace("users/", "");
        itemScript.SetFriendRequestData(userEntry.Value, userId, AcceptFriendRequest, RejectFriendRequest);
        Debug.Log("친구 요청 생성됨");
    }
}
