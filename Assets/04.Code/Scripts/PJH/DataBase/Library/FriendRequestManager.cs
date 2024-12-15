using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class FriendRequestManager
{
    #region FriendList

    /// <summary>
    /// 특정 사용자가 대상 사용자에게 친구 요청을 보냅니다.
    /// </summary>
    /// <param name="userId">요청을 보낸 사용자의 ID</param>
    /// <param name="targetUserId">요청을 받을 사용자의 ID</param>
    public static async UniTask SendFriendRequestAsync(string userId, string targetUserId)
    {
        try
        {
            var outgoingUpdates = new Dictionary<string, object> { { targetUserId, true } };
            await RealtimeDatabase.UpdateDataAsync($"users/{userId}/friendRequests/outgoing", outgoingUpdates);

            var incomingUpdates = new Dictionary<string, object> { { userId, true } };
            await RealtimeDatabase.UpdateDataAsync($"users/{targetUserId}/friendRequests/incoming", incomingUpdates);
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 요청 전송 실패: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 특정 사용자가 닉네임을 사용하여 친구 요청을 보냅니다.
    /// </summary>
    /// <param name="userId">요청을 보낸 사용자의 ID</param>
    /// <param name="targetUsername">요청을 받을 사용자의 닉네임</param>
    public static async UniTask SendFriendRequestByUsernameAsync(string userId, string targetUsername)
    {
        try
        {
            var targetUserId = await RealtimeDatabase.FindUserIdByUsernameAsync(targetUsername);
            if (string.IsNullOrEmpty(targetUserId))
            {
                throw new Exception("해당 닉네임을 가진 사용자가 없습니다.");
            }

            await SendFriendRequestAsync(userId, targetUserId);
        }
        catch (Exception e)
        {
            Debug.LogError($"닉네임으로 친구 요청 전송 실패: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 특정 사용자의 친구 요청을 수락합니다.
    /// </summary>
    /// <param name="userId">요청을 수락하는 사용자의 ID</param>
    /// <param name="requesterId">친구 요청을 보낸 사용자의 ID</param>
    public static async UniTask AcceptFriendRequestAsync(string userId, string requesterId)
    {
        try
        {
            var userUpdate = new Dictionary<string, object> { { requesterId, true } };
            await RealtimeDatabase.UpdateDataAsync($"users/{userId}/friends", userUpdate);

            var requesterUpdate = new Dictionary<string, object> { { userId, true } };
            await RealtimeDatabase.UpdateDataAsync($"users/{requesterId}/friends", requesterUpdate);

            await RemoveFriendRequestAsync(userId, requesterId);
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 요청 수락 실패: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 특정 사용자의 친구 요청을 거절하거나 삭제합니다.
    /// </summary>
    /// <param name="userId">요청을 거절하는 사용자의 ID</param>
    /// <param name="requesterId">친구 요청을 보낸 사용자의 ID</param>
    public static async UniTask RemoveFriendRequestAsync(string userId, string requesterId)
    {
        try
        {
            await RealtimeDatabase.DeleteDataAsync($"users/{userId}/friendRequests/incoming/{requesterId}");
            await RealtimeDatabase.DeleteDataAsync($"users/{requesterId}/friendRequests/outgoing/{userId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 요청 삭제 실패: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 특정 사용자의 친구 목록을 가져옵니다.
    /// </summary>
    /// <param name="userId">친구 목록을 가져올 사용자의 ID</param>
    /// <returns>친구 ID 목록</returns>
    public static async UniTask<List<string>> GetFriendsAsync(string userId)
    {
        try
        {
            var friendDict = await RealtimeDatabase.ReadDataAsync<Dictionary<string, bool>>($"users/{userId}/friends");
            return friendDict != null ? new List<string>(friendDict.Keys) : new List<string>();
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 목록 조회 실패: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 특정 친구의 온라인 상태와 마지막 로그인 시간을 가져옵니다.
    /// </summary>
    /// <param name="friendId">온라인 상태를 확인할 친구의 ID</param>
    /// <returns>친구의 사용자 정보</returns>
    public static async UniTask<User> GetFriendStatusAsync(string friendId)
    {
        try
        {
            return await RealtimeDatabase.ReadDataAsync<User>($"users/{friendId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"친구 상태 조회 실패: {e.Message}");
            throw;
        }
    }

    #endregion
}
