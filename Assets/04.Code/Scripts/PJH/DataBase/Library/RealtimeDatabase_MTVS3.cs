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
    /// 닉네임을 사용하여 사용자 ID를 검색합니다.
    /// </summary>
    /// <param name="username">검색할 닉네임</param>
    /// <param name="onSuccess">일치하는 사용자 ID를 반환하는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void FindUserIdByUsername(string username, Action<string> onSuccess, Action<Exception> onFailure)
    {
        if (string.IsNullOrEmpty(username))
        {
            onFailure?.Invoke(new ArgumentException("닉네임은 비어 있을 수 없습니다."));
            return;
        }

        DatabaseReference usernamesRef = FirebaseDatabase.DefaultInstance.GetReference("usernames");
        usernamesRef.Child(username).GetValueAsync().ContinueWith(task => 
        {
            if (task.IsFaulted)
            {
                onFailure?.Invoke(new Exception($"닉네임 검색 중 오류 발생: {task.Exception}", task.Exception));
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                string userId = snapshot.Value.ToString();
                onSuccess?.Invoke(userId);
            }
            else
            {
                onSuccess?.Invoke(null); // 해당 닉네임을 가진 사용자가 없음을 null로 표시
            }
        });
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
    /// 이름을 사용해 사용자 ID를 검색합니다.
    /// </summary>
    /// <param name="name">검색할 이름</param>
    /// <param name="onSuccess">일치하는 사용자 목록을 반환하는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void FindUserIdsByName(string name, Action<Dictionary<string, User>> onSuccess, Action<Exception> onFailure)
    {
        ReadData<Dictionary<string, User>>($"users",
            onSuccess: users =>
            {
                Dictionary<string, User> matchedUsers = new Dictionary<string, User>();

                foreach (var userEntry in users)
                {
                    if (userEntry.Value.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedUsers[userEntry.Key] = userEntry.Value;
                    }
                }

                if (matchedUsers.Count > 0)
                {
                    onSuccess(matchedUsers);
                }
                else
                {
                    onFailure(new Exception("해당 이름을 가진 사용자를 찾을 수 없습니다."));
                }
            },
            onFailure);
    }
    
    /// <summary>
    /// 사용자 ID를 사용해 닉네임을 검색합니다.
    /// </summary>
    /// <param name="userId">검색할 사용자 ID</param>
    /// <param name="onSuccess">사용자의 닉네임을 반환하는 콜백</param>
    /// <param name="onFailure">작업 실패 시 호출되는 콜백</param>
    public static void FindNameById(string userId, Action<string> onSuccess, Action<Exception> onFailure)
    {
        ReadData<User>($"users/{userId}",
            onSuccess: user =>
            {
                if (user != null && !string.IsNullOrEmpty(user.name))
                {
                    onSuccess(user.name);
                }
                else
                {
                    onFailure(new Exception("해당 ID를 가진 사용자를 찾을 수 없습니다."));
                }
            },
            onFailure);
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
