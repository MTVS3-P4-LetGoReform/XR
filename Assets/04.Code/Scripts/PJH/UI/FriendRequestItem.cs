using UnityEngine;
using UnityEngine.UI;

public class FriendRequestItem : MonoBehaviour
{
    public Text friendNameText;  // 친구 이름 표시 텍스트
    public Image profileImage;  // 친구 프로필 이미지
    public Button acceptButton;  // 수락 버튼
    public Button rejectButton;  // 거절 버튼

    private string requesterId;  // 요청자 ID
    private string currentUserId = "user123";  // 로그인한 유저의 ID (Firebase Authentication에서 제공받은 토큰)

    /// <summary>
    /// 친구 요청 데이터를 설정합니다.
    /// </summary>
    public void SetFriendRequestData(User friend, string requesterId)
    {
        this.requesterId = requesterId;
        friendNameText.text = friend.name;
        // 여기서 프로필 이미지를 불러와 적용할 수 있음 (예시에서는 기본 이미지로 사용)
        profileImage.sprite = Resources.Load<Sprite>(friend.profileImage);  // 기본 이미지를 로드하여 적용

        acceptButton.onClick.AddListener(() => AcceptFriendRequest());
        rejectButton.onClick.AddListener(() => RejectFriendRequest());
    }

    /// <summary>
    /// 친구 요청 수락 처리
    /// </summary>
    private void AcceptFriendRequest()
    {
        FriendRequestManager.AcceptFriendRequest(currentUserId, requesterId,
            onSuccess: () =>
            {
                Debug.Log("친구 요청 수락 성공");
                Destroy(gameObject);  // 요청 수락 후 UI 제거
            },
            onFailure: exception => Debug.LogError($"친구 요청 수락 실패: {exception.Message}"));
    }

    /// <summary>
    /// 친구 요청 거절 처리
    /// </summary>
    private void RejectFriendRequest()
    {
        FriendRequestManager.RemoveFriendRequest(currentUserId, requesterId,
            onSuccess: () =>
            {
                Debug.Log("친구 요청 거절 성공");
                Destroy(gameObject);  // 요청 거절 후 UI 제거
            },
            onFailure: exception => Debug.LogError($"친구 요청 거절 실패: {exception.Message}"));
    }
}