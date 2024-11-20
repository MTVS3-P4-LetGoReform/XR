using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FriendRequestUIManager : MonoBehaviourSingleton<FriendRequestUIManager>
{
    public Transform friendRequestListParent;  // 친구 요청을 보여줄 부모 오브젝트 (스크롤뷰의 Content)
    public GameObject friendRequestItemPrefab;  // 친구 요청 프리팹
    public InputField usernameInputField; // 닉네임 입력 필드
    public Button sendRequestButton; // 친구 요청 전송 버튼
    private string _currentUserId; 

    private void Start()
    {
        _currentUserId = UserData.Instance.UserName;
        
        ListenForFriendRequests();

        // 닉네임으로 친구 요청 전송 버튼 이벤트
        sendRequestButton.onClick.AddListener(() => 
        {
            string targetUsername = usernameInputField.text;
            if (!string.IsNullOrEmpty(targetUsername))
            {
                SendFriendRequestByUsername(targetUsername);
            }
            else
            {
                Debug.LogWarning("닉네임을 입력하세요.");
            }
        });
    }

    /// <summary>
    /// 친구 요청 수신 대기
    /// </summary>
    private void ListenForFriendRequests()
    {
        string path = $"users/{_currentUserId}/friendRequests/incoming";

        // 데이터 변경 수신
        RealtimeDatabase.ListenForDataChanges<Dictionary<string, bool>>(path, OnFriendRequestReceived, OnFriendRequestError);
    }

    /// <summary>
    /// 친구 요청 수신 시 UI에 요청 정보를 표시합니다.
    /// </summary>
    private void OnFriendRequestReceived(Dictionary<string, bool> friendRequests)
    {
        foreach (Transform child in friendRequestListParent)
        {
            Destroy(child.gameObject);
        }

        if (friendRequests?.Count > 0)
        {
            List<string> requesterPaths = new List<string>();
            foreach (var request in friendRequests)
            {
                requesterPaths.Add($"users/{request.Key}");
            }
            
            RealtimeDatabase.ReadMultipleData<User>(requesterPaths,
                onSuccess: users =>
                {
                    foreach (var userEntry in users)
                    {
                        GameObject friendRequestItem = Instantiate(friendRequestItemPrefab, friendRequestListParent);
                        FriendRequestItem itemScript = friendRequestItem.GetComponent<FriendRequestItem>();
                        itemScript.SetFriendRequestData(userEntry.Value, userEntry.Key, AcceptFriendRequest, RejectFriendRequest);
                    }
                },
                onFailure: exception =>
                {
                    Debug.LogError($"친구 요청자 정보 일괄 로드 실패: {exception.Message}");
                });
        }
    }
    

    /// <summary>
    /// 친구 요청 수신 오류 처리
    /// </summary>
    private void OnFriendRequestError(Exception exception)
    {
        Debug.LogError($"친구 요청 수신 중 오류 발생: {exception.Message}");
    }

    /// <summary>
    /// 닉네임으로 친구 요청 보내기
    /// </summary>
    /// <param name="targetUsername">대상 사용자의 닉네임</param>
    private void SendFriendRequestByUsername(string targetUsername)
    {
        FriendRequestManager.SendFriendRequestByUsername(_currentUserId, targetUsername,
            onSuccess: () => Debug.Log($"닉네임 {targetUsername}로 친구 요청 전송 성공"),
            onFailure: exception => Debug.LogError($"친구 요청 전송 실패: {exception.Message}"));
    }

    /// <summary>
    /// 친구 요청 수락
    /// </summary>
    /// <param name="requesterId">친구 요청을 보낸 사용자의 ID</param>
    private void AcceptFriendRequest(string requesterId)
    {
        FriendRequestManager.AcceptFriendRequest(_currentUserId, requesterId,
            onSuccess: () => Debug.Log("친구 요청 수락 성공"),
            onFailure: exception => Debug.LogError($"친구 요청 수락 실패: {exception.Message}"));
    }

    /// <summary>
    /// 친구 요청 거절
    /// </summary>
    /// <param name="requesterId">친구 요청을 보낸 사용자의 ID</param>
    private void RejectFriendRequest(string requesterId)
    {
        FriendRequestManager.RemoveFriendRequest(_currentUserId, requesterId,
            onSuccess: () => Debug.Log("친구 요청 거절 성공"),
            onFailure: exception => Debug.LogError($"친구 요청 거절 실패: {exception.Message}"));
    }
}
