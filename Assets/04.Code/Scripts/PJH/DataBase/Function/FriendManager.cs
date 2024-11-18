using Firebase.Functions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FriendManager : MonoBehaviour
{
    private FirebaseFunctions functions;

    private void Start()
    {
        functions = FirebaseFunctions.DefaultInstance;
    }

    public Task SendFriendRequest(string receiverId)
    {
        var data = new Dictionary<string, object>
        {
            { "receiverId", receiverId }
        };
        
        return functions.GetHttpsCallable("sendFriendRequest").CallAsync(data).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("친구 요청 전송 성공");
            }
            else
            {
                Debug.LogError("친구 요청 전송 실패: " + task.Exception?.Message);
            }
        });
    }

    public Task AcceptFriendRequest(string receiverId, string senderId)
    {
        var data = new Dictionary<string, object>
        {
            { "toUserID", receiverId },
            { "fromUserID", senderId },
            { "status", "accepted" }
        };
        
        return functions.GetHttpsCallable("handleFriendRequestUpdate").CallAsync(data).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("친구 요청 수락 성공");
            }
            else
            {
                Debug.LogError("친구 요청 수락 실패: " + task.Exception?.Message);
            }
        });
    }

    public Task DeclineFriendRequest(string receiverId, string senderId)
    {
        var data = new Dictionary<string, object>
        {
            { "toUserID", receiverId },
            { "fromUserID", senderId },
            { "status", "declined" }
        };
        
        return functions.GetHttpsCallable("handleFriendRequestUpdate").CallAsync(data).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("친구 요청 거절 성공");
            }
            else
            {
                Debug.LogError("친구 요청 거절 실패: " + task.Exception?.Message);
            }
        });
    }
}
