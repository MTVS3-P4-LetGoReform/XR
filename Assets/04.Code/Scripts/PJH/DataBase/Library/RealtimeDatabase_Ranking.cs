using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static partial class RealtimeDatabase
{
    // 점수 업데이트
    public static void UpdateScore(string modelId, string username, int score)
    {
        try
        {
            var updates = new Dictionary<string, object>
            {
                { "score", score++ }
            };

            RealtimeDatabase.UpdateData($"models/{modelId}",updates);
            Debug.Log($"좋아요 업데이트 됨 - 모델이름 : {modelId}, 좋아요 수 : {score}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating score: {e.Message}");
        }
    }
    
    // 상위 랭킹 조회
    public static async UniTask<List<RankingEntry>> GetTopRankings(int limit = 9)
    {
        List<RankingEntry> rankings = new List<RankingEntry>();
        
        try
        {
            var snapshot = await databaseReference.Child("models")
                .OrderByChild("score")
                .LimitToLast(limit)
                .GetValueAsync();

            int rank = 1;
            // Firebase에서 내림차순으로 데이터를 가져오기 위해 역순으로 처리
            var tempList = new List<RankingEntry>();
            
            foreach (var child in snapshot.Children)
            {
                var modelScore = JsonConvert.DeserializeObject<Model>(child.GetRawJsonValue());
                string userName = "";
                RealtimeDatabase.FindNameById(modelScore.creator_id, onSuccess: (name) =>
                    {
                        userName = name;
                        Debug.Log($"Found user name: {name}");
                    },
                    onFailure: (error) =>
                    {
                        Debug.LogError($"유저를 찾기 못했습니다. : {error.Message}");
                    });

                tempList.Add(new RankingEntry
                {
                    rank = rank++,
                    username = userName,
                    score = modelScore.score,
                    modelName = modelScore.prompt_ko,
                    modelImageName = modelScore.select_image_name,
                    modelId = modelScore.id
                });
            }

            // 점수 내림차순으로 정렬
            tempList.Sort((a, b) => b.score.CompareTo(a.score));
            
            // 순위 재할당
            for (int i = 0; i < tempList.Count; i++)
            {
                tempList[i].rank = i + 1;
            }

            rankings = tempList;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching rankings: {e.Message}");
        }

        return rankings;
    }

    // 특정 Model의 랭킹 조회
    public static async UniTask<RankingEntry> GetModelRank(string modelId)
    {
        try
        {
            var allScores = new List<KeyValuePair<string, Model>>();
            var snapshot = await databaseReference.Child("models").GetValueAsync();

            foreach (var child in snapshot.Children)
            {
                var userScore = JsonConvert.DeserializeObject<Model>(child.GetRawJsonValue());
                allScores.Add(new KeyValuePair<string, Model>(child.Key, userScore));
            }

            // 점수로 정렬
            allScores.Sort((a, b) => b.Value.score.CompareTo(a.Value.score));

            // 사용자의 순위 찾기
            for (int i = 0; i < allScores.Count; i++)
            {
                if (allScores[i].Key == modelId)
                {
                    string userName = "";
                    RealtimeDatabase.FindNameById(allScores[i].Value.creator_id, onSuccess: (name) =>
                        {
                            userName = name;
                            Debug.Log($"Found user name: {name}");
                        },
                        onFailure: (error) =>
                        {
                            Debug.LogError($"유저를 찾기 못했습니다. : {error.Message}");
                        });

                    return new RankingEntry
                    {
                        rank = i + 1,
                        username = userName,
                        score = allScores[i].Value.score
                    };
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching user rank: {e.Message}");
        }

        return null;
    }
}