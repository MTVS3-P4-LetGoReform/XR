using Firebase;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static partial class RealtimeDatabase 
{
    /// <summary>
    /// 닉네임 중복을 확인하고 사용자를 생성합니다.
    /// </summary>
    public static async UniTask CreateUserWithUsernameAsync(string userId, User user)
    {
        try 
        {
            var existingUserId = await ReadDataAsync<string>($"usernames/{user.name}");
            
            if (!string.IsNullOrEmpty(existingUserId))
            {
                throw new Exception("이미 사용 중인 닉네임입니다.");
            }
            
            await CreateUserAsync(userId, user);
            await CreateDataAsync($"usernames/{user.name}", userId);
        }
        catch (Exception e)
        {
            Debug.LogError($"사용자 생성 실패: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 특정 유저를 생성합니다.
    /// </summary>
    public static async UniTask CreateUserAsync(string userId, User user)
    {
        await CreateDataAsync($"users/{userId}", user);
    }

    /// <summary>
    /// 유저 데이터를 읽어옵니다.
    /// </summary>
    public static async UniTask<User> GetUserAsync(string userId)
    {
        return await ReadDataAsync<User>($"users/{userId}");
    }

    /// <summary>
    /// 닉네임을 사용하여 사용자 ID를 검색합니다.
    /// </summary>
    public static async UniTask<string> FindUserIdByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("닉네임은 비어 있을 수 없습니다.");
        }

        try
        {
            return await ReadDataAsync<string>($"usernames/{username}");
        }
        catch (Exception e)
        {
            Debug.LogError($"닉네임으로 사용자 검색 실패: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 유저 데이터를 업데이트합니다.
    /// </summary>
    public static async UniTask UpdateUserAsync(string userId, Dictionary<string, object> updates)
    {
        await UpdateDataAsync($"users/{userId}", updates);
    }

    /// <summary>
    /// 유저 데이터를 삭제합니다.
    /// </summary>
    public static async UniTask DeleteUserAsync(string userId)
    {
        await DeleteDataAsync($"users/{userId}");
    }

    /// <summary>
    /// 이름을 사용해 사용자 ID를 검색합니다.
    /// </summary>
    public static async UniTask<Dictionary<string, User>> FindUserIdsByNameAsync(string name)
    {
        var users = await ReadDataAsync<Dictionary<string, User>>("users");
        var matchedUsers = new Dictionary<string, User>();

        foreach (var userEntry in users)
        {
            if (userEntry.Value.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                matchedUsers[userEntry.Key] = userEntry.Value;
            }
        }

        if (matchedUsers.Count == 0)
        {
            throw new Exception("해당 이름을 가진 사용자를 찾을 수 없습니다.");
        }

        return matchedUsers;
    }

    /// <summary>
    /// 특정 유저의 영지 데이터를 생성하거나 업데이트합니다.
    /// </summary>
    public static async UniTask SetUserLandAsync(string userId, UserLand userLand)
    {
        await CreateDataAsync($"user_land/{userId}", userLand);
    }

    /// <summary>
    /// 특정 유저의 영지 정보를 설정합니다.
    /// </summary>
    public static async UniTask SetUserLandInfoAsync(string userId, LandInfo landInfo)
    {
        await CreateDataAsync($"user_land/{userId}/landInfo", landInfo);
    }

    /// <summary>
    /// 특정 유저의 영지 데이터를 읽어옵니다.
    /// </summary>
    public static async UniTask<UserLand> GetUserLandAsync(string userId)
    {
        return await ReadDataAsync<UserLand>($"user_land/{userId}");
    }

    /// <summary>
    /// 특정 유저의 영지에 오브젝트를 추가합니다.
    /// </summary>
    public static async UniTask AddObjectToUserLandAsync(string userId, LandObject landObject)
    {
        var userLand = await GetUserLandAsync(userId);
        
        if (userLand == null)
        {
            string userName = await FindNameByIdAsync(userId);
            userLand = new UserLand(userName);
        }

        userLand.AddObject(landObject);
        await SetUserLandAsync(userId, userLand);
    }

    /// <summary>
    /// AI 모델을 추가합니다.
    /// </summary>
    public static async UniTask AddAiModelAsync(string userId, Model model)
    {
        var modelList = await GetModelListAsync(userId);
        
        if (modelList == null) 
            modelList = new ModelList();

        bool added = modelList.AddModel(model);
        if (!added)
        {
            Debug.LogWarning($"사용자 {userId}에게 이미 존재하는 모델 ID: {model.id}");
            throw new Exception("이미 존재하는 모델입니다.");
        }

        await CreateDataAsync($"users/{userId}/models", modelList);
    }

    /// <summary>
    /// 모델 목록을 가져옵니다.
    /// </summary>
    public static async UniTask<ModelList> GetModelListAsync(string userId)
    {
        return await ReadDataAsync<ModelList>($"users/{userId}/models");
    }

    /// <summary>
    /// 특정 ModelId에 해당하는 Model 데이터를 가져와 UserId 아래에 저장합니다.
    /// </summary>
    public static async UniTask CopyModelToUserAsync(string userId, string modelId)
    {
        var model = await ReadDataAsync<Model>($"models/{modelId}");
        
        if (model == null)
        {
            Debug.LogError($"Model ID '{modelId}'에 해당하는 모델을 찾을 수 없습니다.");
            throw new Exception("모델을 찾을 수 없습니다.");
        }

        await CreateDataAsync($"users/{userId}/models/{modelId}", model);
    }
    
    /// <summary>
    /// 사용자 ID를 사용해 닉네임을 검색합니다.
    /// </summary>
    /// <param name="userId">검색할 사용자 ID</param>
    /// <returns>사용자의 닉네임</returns>
    public static async UniTask<string> FindNameByIdAsync(string userId)
    {
        try
        {
            var user = await ReadDataAsync<User>($"users/{userId}");
            return user.name;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

}
