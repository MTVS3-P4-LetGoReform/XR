using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestItem : MonoBehaviour
{
    public TMP_Text friendNameText;
    public Button acceptButton;
    public Button rejectButton;

    private string _requesterId;

    /// <summary>
    /// 친구 요청 데이터 설정
    /// </summary>
    /// <param name="friend">요청자의 정보</param>
    /// <param name="requesterId">요청자의 ID</param>
    /// <param name="onAccept">수락 콜백</param>
    /// <param name="onReject">거절 콜백</param>
    public void SetFriendRequestData(User friend, string requesterId, Action<string> onAccept, Action<string> onReject)
    {
        friendNameText.text = friend.name;
        _requesterId = requesterId;

        Debug.Log("reqId : " + requesterId);
        acceptButton.onClick.AddListener(() => onAccept(_requesterId));
        rejectButton.onClick.AddListener(() => onReject(_requesterId));
    }
}