using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FriendRequestUIManager : MonoBehaviour
{
    public Transform friendRequestListParent;  // 친구 요청을 보여줄 부모 오브젝트 (스크롤뷰의 Content)
    public GameObject friendRequestItemPrefab;  // 친구 요청 프리팹
    private string currentUserId = "user123";  // 로그인한 유저의 ID (Firebase Authentication에서 제공받은 토큰)

    private void Start()
    {
        ListenForFriendRequests();
    }

    /// <summary>
    /// 친구 요청 수신 대기
    /// </summary>
    private void ListenForFriendRequests()
    {
        // 친구 요청 수신 경로
        string path = $"users/{currentUserId}/friendRequests/incoming";

        // 데이터 변경 수신
        RealtimeDatabase.ListenForDataChanges<Dictionary<string, bool>>(path, OnFriendRequestReceived, OnFriendRequestError);
    }

    /// <summary>
    /// 친구 요청 수신 시 처리
    /// </summary>
    /// <param name="friendRequests">받은 친구 요청 목록</param>
    private void OnFriendRequestReceived(Dictionary<string, bool> friendRequests)
    {
        // 기존 UI 정리
        foreach (Transform child in friendRequestListParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 요청에 대한 UI 생성
        if (friendRequests != null)
        {
            foreach (var request in friendRequests)
            {
                string requesterId = request.Key;

                // 요청자 정보 로드
                RealtimeDatabase.GetUser(requesterId, user =>
                {
                    // 친구 요청 프리팹 인스턴스 생성
                    GameObject friendRequestItem = Instantiate(friendRequestItemPrefab, friendRequestListParent);
                    FriendRequestItem itemScript = friendRequestItem.GetComponent<FriendRequestItem>();
                    itemScript.SetFriendRequestData(user, requesterId);
                },
                exception => Debug.LogError($"친구 요청자 정보 로드 실패: {exception.Message}"));
            }
        }
    }

    /// <summary>
    /// 친구 요청 수신 오류 처리
    /// </summary>
    private void OnFriendRequestError(Exception exception)
    {
        Debug.LogError($"친구 요청 수신 중 오류 발생: {exception.Message}");
    }
}
