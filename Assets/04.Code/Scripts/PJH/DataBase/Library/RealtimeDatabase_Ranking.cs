using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static partial class RealtimeDatabase
{
    // 점수 업데이트
    public static async UniTask UpdateScore(string modelId, int score)
    {
        try
        {
            var currentScore = score + 1;
            var updates = new Dictionary<string, object>
            {
                { "score", currentScore }
            };

            await UpdateDataAsync($"models/{modelId}",updates);
            Debug.Log($"좋아요 업데이트 됨 - 모델이름 : {modelId}, 좋아요 수 : {currentScore}");
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

        await EnsureInitializedAsync(onInitialized: () => { },
            onFailure: (error) => { Debug.LogError($"Firebase 초기화 실패: {error.Message}"); });

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
                try
                {
                    userName = await FindNameByIdAsync(modelScore.creator_id);
                    Debug.Log($"닉네임 조회 성공: {userName}");
                }
                catch (Exception e)
                {
                    userName = "알 수 없는 사용자";
                    Debug.LogWarning($"닉네임 조회 실패: {e.Message}");
                }

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
        RankingEntry result = null;

        await EnsureInitializedAsync(
            onInitialized: () => {Debug.Log($"Firebase 초기화"); },
            onFailure: (error) => { Debug.LogError($"Firebase 초기화 실패: {error.Message}"); });

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

            // 모델의 순위 찾기
            for (int i = 0; i < allScores.Count; i++)
            {
                if (allScores[i].Key == modelId)
                {
                    // Null 체크 및 반환값 설정
                    result = new RankingEntry
                    {
                        rank = i + 1,
                        modelName = allScores[i].Value?.prompt_ko ?? "알 수 없음",
                        score = allScores[i].Value?.score ?? 0
                    };
                    Debug.Log($"Rank: {result.rank}, UserName: {result.modelName}, Score: {result.score}");
                    return result; // Rank를 찾았으면 해당 순위 반환
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching user rank: {e.Message}");
        }

        return result; // EnsureInitialized가 끝난 후 반환
    }


}