using Cysharp.Threading.Tasks;
using UnityEngine;

public class FriendManagerTests : MonoBehaviour
{
    private FriendManager friendManager;
    private string testToUserID = "testToUserID";
    private string testFromUserID = "testFromUserID";

    private async void Start()
    {
        friendManager = new GameObject("FriendManager").AddComponent<FriendManager>();

        // 1. 친구 요청 보내기 테스트
        await SendFriendRequestTest();

        // 2. 친구 요청 수락 테스트
        await AcceptFriendRequestTest();

        // 3. 친구 요청 거절 테스트
        await DeclineFriendRequestTest();

        Debug.Log("모든 테스트 완료");
    }

    private async UniTask SendFriendRequestTest()
    {
        Debug.Log("친구 요청 테스트 시작");
        try
        {
            await friendManager.SendFriendRequest(testToUserID);
            Debug.Log("친구 요청 전송 성공");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"친구 요청 전송 실패: {ex.Message}");
        }
    }

    private async UniTask AcceptFriendRequestTest()
    {
        Debug.Log("친구 요청 수락 테스트 시작");

        // 친구 요청을 먼저 보낸 후 수락
        try
        {
            await friendManager.SendFriendRequest(testToUserID);
            Debug.Log("친구 요청 전송 성공");

            await friendManager.AcceptFriendRequest(testToUserID, testFromUserID);
            Debug.Log("친구 요청 수락 성공");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"친구 요청 수락 실패: {ex.Message}");
        }
    }

    private async UniTask DeclineFriendRequestTest()
    {
        Debug.Log("친구 요청 거절 테스트 시작");

        // 친구 요청을 먼저 보낸 후 거절
        try
        {
            await friendManager.SendFriendRequest(testToUserID);
            Debug.Log("친구 요청 전송 성공");

            await friendManager.DeclineFriendRequest(testToUserID, testFromUserID);
            Debug.Log("친구 요청 거절 성공");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"친구 요청 거절 실패: {ex.Message}");
        }
    }
}