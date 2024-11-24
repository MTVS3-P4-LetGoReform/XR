using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TheGoatManager : MonoBehaviour
{
    public GoatItem goatItem;
    public Transform parentTransform;
    
    // 점수 업데이트
    public async void ClickScore(string modelId, string username)
    {
        var userRank = await RealtimeDatabase.GetModelRank(modelId);
        RealtimeDatabase.UpdateScore(modelId, username, userRank.score);
    }

    // 상위 랭킹 조회
    private async void GetTopRankings()
    {
        List<RankingEntry> topRankings = await RealtimeDatabase.GetTopRankings();
        foreach (var entry in topRankings)
        {
            var gtObject = Instantiate(goatItem, parentTransform);
            gtObject.SetGoatData(entry);
            
            Debug.Log($"Rank {entry.rank}: {entry.username} - {entry.score}");
        }
    }

    // 특정 모델의 랭킹 조회
    public async void GetUserRank(string modelId)
    {
        RankingEntry userRank = await RealtimeDatabase.GetModelRank(modelId);
        if (userRank != null)
        {
            Debug.Log($"{userRank.username}'s rank: {userRank.rank} (Score: {userRank.score})");
        }
    }
}