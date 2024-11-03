using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class RealtimeDatabase
{
    /// <summary>
    /// 특정 유저를 생성합니다.
    /// </summary>
    public static void CreateUser(string userId, User user, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        CreateData($"users/{userId}", user, onSuccess, onFailure);
    }

    /// <summary>
    /// 유저 데이터를 읽어옵니다.
    /// </summary>
    public static void GetUser(string userId, Action<User> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData($"users/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 유저 데이터를 업데이트합니다.
    /// </summary>
    public static void UpdateUser(string userId, Dictionary<string, object> updates, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        UpdateData($"users/{userId}", updates, onSuccess, onFailure);
    }


    /// <summary>
    /// 유저 데이터를 삭제합니다.
    /// </summary>
    public static void DeleteUser(string userId, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        DeleteData($"users/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 영지 데이터를 생성하거나 업데이트합니다.
    /// </summary>
    public static void SetUserLand(string userId, UserLand userLand, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        CreateData($"user_land/{userId}", userLand, onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 영지 데이터를 읽어옵니다.
    /// </summary>
    public static void GetUserLand(string userId, Action<UserLand> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData($"user_land/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 영지에서 오브젝트를 추가합니다.
    /// </summary>
    public static void AddObjectToUserLand(string userId, LandObject landObject, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        GetUserLand(userId, userLand =>
        {
            if (userLand == null) userLand = new UserLand();
            userLand.AddObject(landObject);

            SetUserLand(userId, userLand, onSuccess, onFailure);
        }, onFailure);
    }

    /// <summary>
    /// 특정 유저의 친구 목록에 친구를 추가합니다.
    /// </summary>
    public static void AddFriend(string userId, Friend friend, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        GetFriendList(userId, friendList =>
        {
            if (friendList == null) friendList = new FriendList();
            friendList.AddFriend(friend);

            CreateData($"friend_list/{userId}", friendList, onSuccess, onFailure);
        }, onFailure);
    }

    /// <summary>
    /// 특정 유저의 친구 목록을 읽어옵니다.
    /// </summary>
    public static void GetFriendList(string userId, Action<FriendList> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData($"friend_list/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 친구 목록에서 친구를 삭제합니다.
    /// </summary>
    public static void RemoveFriend(string userId, string friendId, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        GetFriendList(userId, friendList =>
        {
            if (friendList != null && friendList.friends.ContainsKey(friendId))
            {
                friendList.friends.Remove(friendId);

                CreateData($"friend_list/{userId}", friendList, onSuccess, onFailure);
            }
            else
            {
                Debug.LogError("친구 삭제 실패: 친구 ID를 찾을 수 없습니다.");
                onFailure?.Invoke(new Exception("친구 ID를 찾을 수 없습니다."));
            }
        }, onFailure);
    }
}
