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
    /// <param name="userId">생성할 유저의 ID입니다.</param>
    /// <param name="user">생성할 유저 데이터 객체입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
    public static void CreateUser(string userId, User user, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        CreateData($"users/{userId}", user, onSuccess, onFailure);
    }

    /// <summary>
    /// 유저 데이터를 읽어옵니다.
    /// </summary>
    /// <param name="userId">읽어올 유저의 ID입니다.</param>
    /// <param name="onSuccess">데이터를 성공적으로 읽어오면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">데이터 읽기에 실패하면 호출되는 콜백입니다.</param>
    public static void GetUser(string userId, Action<User> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData($"users/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 유저 데이터를 업데이트합니다.
    /// </summary>
    /// <param name="userId">업데이트할 유저의 ID입니다.</param>
    /// <param name="updates">업데이트할 데이터의 키-값 쌍입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
    public static void UpdateUser(string userId, Dictionary<string, object> updates, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        UpdateData($"users/{userId}", updates, onSuccess, onFailure);
    }

    /// <summary>
    /// 유저 데이터를 삭제합니다.
    /// </summary>
    /// <param name="userId">삭제할 유저의 ID입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
    public static void DeleteUser(string userId, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        DeleteData($"users/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 영지 데이터를 생성하거나 업데이트합니다.
    /// </summary>
    /// <param name="userId">영지 데이터를 생성 또는 업데이트할 유저의 ID입니다.</param>
    /// <param name="userLand">저장할 영지 데이터 객체입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
    public static void SetUserLand(string userId, UserLand userLand, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        CreateData($"user_land/{userId}", userLand, onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 영지 데이터를 읽어옵니다.
    /// </summary>
    /// <param name="userId">영지 데이터를 읽어올 유저의 ID입니다.</param>
    /// <param name="onSuccess">데이터를 성공적으로 읽어오면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">데이터 읽기에 실패하면 호출되는 콜백입니다.</param>
    public static void GetUserLand(string userId, Action<UserLand> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData($"user_land/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 영지에서 오브젝트를 추가합니다.
    /// </summary>
    /// <param name="userId">영지에 오브젝트를 추가할 유저의 ID입니다.</param>
    /// <param name="landObject">추가할 오브젝트 데이터입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
    public static void AddObjectToUserLand(string userId, LandObject landObject, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        GetUserLand(userId, userLand =>
        {
            if (userLand == null)
                userLand = new UserLand();
            
            userLand.AddObject(landObject);

            SetUserLand(userId, userLand, onSuccess, onFailure);
        }, onFailure);
    }

    /// <summary>
    /// 특정 유저의 영지 데이터 변경 사항을 실시간으로 수신합니다.
    /// </summary>
    /// <param name="userId">실시간 변경 사항을 수신할 유저의 ID입니다.</param>
    /// <param name="onDataChanged">데이터가 변경되면 호출되는 콜백입니다.</param>
    /// <param name="onError">데이터 수신 중 오류가 발생하면 호출되는 콜백입니다.</param>
    public static void ListenForUserLandChanges(string userId, Action<UserLand> onDataChanged, Action<Exception> onError = null)
    {
        ListenForDataChanges($"user_land/{userId}", onDataChanged, onError);
    }

    /// <summary>
    /// 특정 유저의 친구 목록에 친구를 추가합니다.
    /// </summary>
    /// <param name="userId">친구를 추가할 유저의 ID입니다.</param>
    /// <param name="friend">추가할 친구 객체입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
    public static void AddFriend(string userId, Friend friend, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        GetFriendList(userId, friendList =>
        {
            if (friendList == null) 
                friendList = new FriendList();
           
            friendList.AddFriend(friend);

            CreateData($"friend_list/{userId}", friendList, onSuccess, onFailure);
        }, onFailure);
    }

    /// <summary>
    /// 특정 유저의 친구 목록을 읽어옵니다.
    /// </summary>
    /// <param name="userId">친구 목록을 읽어올 유저의 ID입니다.</param>
    /// <param name="onSuccess">데이터를 성공적으로 읽어오면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">데이터 읽기에 실패하면 호출되는 콜백입니다.</param>
    public static void GetFriendList(string userId, Action<FriendList> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData($"friend_list/{userId}", onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 유저의 친구 목록에서 친구를 삭제합니다.
    /// </summary>
    /// <param name="userId">친구를 삭제할 유저의 ID입니다.</param>
    /// <param name="friendId">삭제할 친구의 ID입니다.</param>
    /// <param name="onSuccess">작업이 성공하면 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업이 실패하면 호출되는 콜백입니다.</param>
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
    
    public static void AddAiModel(string userId, Model model, Action onSuccess = null, Action<Exception> onFailure = null)
    {
        GetModelList(userId, modelList =>
        {
            if (modelList == null) 
                modelList = new ModelList();
            
            bool added = modelList.AddModel(model);
            if (!added)
            {
                Debug.LogWarning($"사용자 {userId}에게 이미 존재하는 모델 ID: {model.id}");
                onFailure?.Invoke(new Exception("이미 존재하는 모델입니다."));
                return;
            }
            
            CreateData($"users/{userId}/models", modelList, onSuccess, onFailure);
        }, onFailure);
    }

    public static void GetModelList(string userId, Action<ModelList> onSuccess, Action<Exception> onFailure = null)
    {
        ReadData($"users/{userId}/models", onSuccess, onFailure);
    }

    /// <summary>
    /// 특정 ModelId에 해당하는 Model 데이터를 가져와 UserId 아래에 저장합니다.
    /// </summary>
    /// <param name="userId">모델을 저장할 유저의 ID입니다.</param>
    /// <param name="modelId">가져올 모델의 ID입니다.</param>
    /// <param name="onSuccess">작업 성공 시 호출되는 콜백입니다.</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백입니다.</param>
    public static void CopyModelToUser(string userId, string modelId, Action onSuccess = null,
        Action<Exception> onFailure = null)
    {
        // models/{modelId} 경로에서 Model 데이터 읽기
        RealtimeDatabase.ReadData<Model>($"models/{modelId}", model =>
        {
            if (model == null)
            {
                Debug.LogError($"Model ID '{modelId}'에 해당하는 모델을 찾을 수 없습니다.");
                onFailure?.Invoke(new Exception("모델을 찾을 수 없습니다."));
                return;
            }

            // 불러온 모델 데이터를 users/{userId}/models/{modelId} 경로에 저장
            RealtimeDatabase.CreateData($"users/{userId}/models/{modelId}", model, onSuccess, onFailure);

        }, onFailure);
    }
}
