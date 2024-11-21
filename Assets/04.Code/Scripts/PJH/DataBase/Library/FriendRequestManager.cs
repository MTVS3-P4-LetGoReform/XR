using System;
using System.Collections.Generic;
using UnityEngine;
using static RealtimeDatabase;
public static class FriendRequestManager
{

    #region FriendList

    /// <summary>
    /// 특정 사용자가 대상 사용자에게 친구 요청을 보냅니다.
    /// </summary>
    /// <param name="userId">요청을 보낸 사용자의 ID</param>
    /// <param name="targetUserId">요청을 받을 사용자의 ID</param>
    /// <param name="onSuccess">작업 성공 시 호출되는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void SendFriendRequest(string userId, string targetUserId, Action onSuccess,
        Action<Exception> onFailure)
    {
        UpdateData($"users/{userId}/friendRequests/outgoing",
            new Dictionary<string, object> { { targetUserId, true } },
            onSuccess: () =>
            {
                UpdateData($"users/{targetUserId}/friendRequests/incoming",
                    new Dictionary<string, object> { { userId, true } },
                    onSuccess,
                    onFailure);
            },
            onFailure);
    }

    /// <summary>
    /// 특정 사용자가 닉네임을 사용하여 친구 요청을 보냅니다.
    /// </summary>
    /// <param name="userId">요청을 보낸 사용자의 ID</param>
    /// <param name="targetUsername">요청을 받을 사용자의 닉네임</param>
    /// <param name="onSuccess">작업 성공 시 호출되는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void SendFriendRequestByUsername(string userId, string targetUsername, Action onSuccess,
        Action<Exception> onFailure)
    {
        FindUserIdByUsername(targetUsername,
            onSuccess: targetUserId =>
            {
                if (!string.IsNullOrEmpty(targetUserId))
                {
                    SendFriendRequest(userId, targetUserId, onSuccess, onFailure);
                }
                else
                {
                    onFailure?.Invoke(new Exception("해당 닉네임을 가진 사용자가 없습니다."));
                }
            },
            onFailure);
    }

    /// <summary>
    /// 특정 사용자의 친구 요청을 수락합니다.
    /// </summary>
    /// <param name="userId">요청을 수락하는 사용자의 ID</param>
    /// <param name="requesterId">친구 요청을 보낸 사용자의 ID</param>
    /// <param name="onSuccess">작업 성공 시 호출되는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void AcceptFriendRequest(string userId, string requesterId, Action onSuccess = null,
        Action<Exception> onFailure = null)
    {
        // 양방향으로 친구 관계를 업데이트합니다.
        UpdateData($"users/{userId}/friends",
            new Dictionary<string, object> { { requesterId, true } },
            onSuccess: () =>
            {
                UpdateData($"users/{requesterId}/friends",
                    new Dictionary<string, object> { { userId, true } },
                    onSuccess: () =>
                    {
                        // 친구 요청을 수락 후 해당 요청을 삭제합니다.
                        RemoveFriendRequest(userId, requesterId, onSuccess, onFailure);
                    },
                    onFailure);
            },
            onFailure);
    }


    /// <summary>
    /// 특정 사용자의 친구 요청을 거절하거나 삭제합니다.
    /// </summary>
    /// <param name="userId">요청을 거절하는 사용자의 ID</param>
    /// <param name="requesterId">친구 요청을 보낸 사용자의 ID</param>
    /// <param name="onSuccess">작업 성공 시 호출되는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void RemoveFriendRequest(string userId, string requesterId, Action onSuccess = null,
        Action<Exception> onFailure = null)
    {
        DeleteData($"users/{userId}/friendRequests/incoming/{requesterId}",
            onSuccess: () =>
            {
                DeleteData($"users/{requesterId}/friendRequests/outgoing/{userId}", onSuccess, onFailure);
            },
            onFailure);
    }

    /// <summary>
    /// 특정 사용자의 친구 목록을 가져옵니다.
    /// </summary>
    /// <param name="userId">친구 목록을 가져올 사용자의 ID</param>
    /// <param name="onSuccess">친구 목록을 성공적으로 가져올 경우 호출되는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void GetFriends(string userId, Action<List<string>> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData<Dictionary<string, bool>>($"users/{userId}/friends",
            onSuccess: friendDict =>
            {
                if (friendDict != null)
                {
                    List<string> friendIds = new List<string>(friendDict.Keys);
                    onSuccess(friendIds);
                }
                else
                {
                    onSuccess(new List<string>());
                }
            },
            onFailure);
    }

    /// <summary>
    /// 특정 친구의 온라인 상태와 마지막 로그인 시간을 가져옵니다.
    /// </summary>
    /// <param name="friendId">온라인 상태를 확인할 친구의 ID</param>
    /// <param name="onSuccess">친구 정보를 성공적으로 가져올 경우 호출되는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void GetFriendStatus(string friendId, Action<User> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData<User>($"users/{friendId}",
            onSuccess: user => { onSuccess(user); },
            onFailure);
    }

    /// <summary>
    /// 닉네임을 사용하여 사용자 ID를 검색합니다.
    /// </summary>
    /// <param name="username">검색할 닉네임</param>
    /// <param name="onSuccess">일치하는 사용자 ID를 반환하는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void FindUserIdByUsername(string username, Action<string> onSuccess, Action<Exception> onFailure)
    {
        ReadData<string>($"usernames/{username}",
            onSuccess: userId =>
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    onSuccess(userId);
                }
                else
                {
                    onFailure(new Exception("해당 닉네임을 가진 사용자를 찾을 수 없습니다."));
                }
            },
            onFailure);
    }

    /// <summary>
    /// 닉네임 중복을 확인하고 사용자 생성 시 닉네임을 등록합니다.
    /// </summary>
    /// <param name="userId">생성할 사용자의 ID</param>
    /// <param name="user">생성할 사용자 객체</param>
    /// <param name="onSuccess">작업 성공 시 호출되는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void CreateUserWithUsername(string userId, User user, Action onSuccess = null,
        Action<Exception> onFailure = null)
    {
        ReadData<string>($"usernames/{user.name}",
            onSuccess: existingUserId =>
            {
                if (!string.IsNullOrEmpty(existingUserId))
                {
                    // 중복된 닉네임 존재
                    onFailure?.Invoke(new Exception("이미 사용 중인 닉네임입니다."));
                }
                else
                {
                    // 유저 생성 및 닉네임 등록
                    CreateUser(userId, user,
                        onSuccess: () => { CreateData($"usernames/{user.name}", userId, onSuccess, onFailure); },
                        onFailure);
                }
            },
            onFailure: exception =>
            {
                // 닉네임이 존재하지 않거나 오류 발생 시 유저 생성
                CreateUser(userId, user,
                    onSuccess: () => { CreateData($"usernames/{user.name}", userId, onSuccess, onFailure); },
                    onFailure);
            });
    }

    #endregion

}
