using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static partial class RealtimeDatabase
{
    /// <summary>
    /// 모델의 점수를 업데이트합니다.
    /// </summary>
    /// <param name="modelId">업데이트할 모델의 ID</param>
    /// <param name="score">현재 점수</param>
    public static async UniTask UpdateScore(string modelId, int score)
    {
        await EnsureInitializedAsync();
        
        try
        {
            var currentScore = score + 1;
            var updates = new Dictionary<string, object>
            {
                { "score", currentScore }
            };

            await UpdateDataAsync($"models/{modelId}", updates);
            Debug.Log($"좋아요 업데이트 됨 - 모델이름 : {modelId}, 좋아요 수 : {currentScore}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating score: {e.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// 상위 랭킹을 조회합니다.
    /// </summary>
    /// <param name="limit">조회할 랭킹의 수</param>
    /// <returns>랭킹 목록</returns>
    public static async UniTask<List<RankingEntry>> GetTopRankings(int limit = 9)
    {
        await EnsureInitializedAsync();
        
        try
        {
            var snapshot = await _databaseReference.Child("models")
                .OrderByChild("score")
                .LimitToLast(limit)
                .GetValueAsync();

            var tempList = new List<RankingEntry>();
            foreach (var child in snapshot.Children)
            {
                var modelScore = JsonConvert.DeserializeObject<Model>(child.GetRawJsonValue());
                
                if (string.IsNullOrEmpty(modelScore.select_image_name) || 
                    string.IsNullOrEmpty(modelScore.prompt_ko))
                {
                    Debug.LogWarning($"필터링된 데이터 - ID: {modelScore.id}");
                    continue;
                }

                string userName;
                try
                {
                    userName = await FindNameByIdAsync(modelScore.creator_id);
                }
                catch (Exception)
                {
                    userName = "알 수 없는 사용자";
                }

                tempList.Add(new RankingEntry
                {
                    rank = 0,
                    username = userName,
                    score = modelScore.score,
                    modelName = modelScore.prompt_ko,
                    selectImageName = modelScore.select_image_name,
                    modelId = modelScore.id
                });
            }

            tempList.Sort((a, b) => b.score.CompareTo(a.score));
            
            for (int i = 0; i < tempList.Count; i++)
            {
                tempList[i].rank = i + 1;
            }

            return tempList;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching rankings: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 특정 모델의 랭킹을 조회합니다.
    /// </summary>
    /// <param name="modelId">조회할 모델의 ID</param>
    /// <returns>모델의 랭킹 정보</returns>
    public static async UniTask<RankingEntry> GetModelRank(string modelId)
    {
        await EnsureInitializedAsync();

        try
        {
            var snapshot = await _databaseReference.Child("models").GetValueAsync();
            
            var allScores = snapshot.Children
                .Select(child => new
                {
                    Key = child.Key,
                    Model = JsonConvert.DeserializeObject<Model>(child.GetRawJsonValue())
                })
                .OrderByDescending(x => x.Model.score)
                .ToList();

            var modelRank = allScores.FindIndex(x => x.Key == modelId);
            if (modelRank != -1)
            {
                var model = allScores[modelRank].Model;
                return new RankingEntry
                {
                    rank = modelRank + 1,
                    modelName = model?.prompt_ko ?? "알 수 없음",
                    score = model?.score ?? 0
                };
            }
            
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching user rank: {e.Message}");
            throw;
        }
    }
}
