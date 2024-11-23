using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TheGoatManager : MonoBehaviour
{
    public GoatItem goatItem;
    public Transform parentTransform;

    private void Start()
    {
        //GetTopRankings();
        RealtimeDatabase.InitializeFirebase();
    }
    
    // 점수 업데이트
    public async void ClickScore(string modelId)
    {
        var userRank = await RealtimeDatabase.GetModelRank(modelId);
        Debug.Log("기존 좋아요 수 : " + userRank.score);
        await RealtimeDatabase.UpdateScore(modelId, userRank.score);
        GetTopRankings();
    }

    // 상위 랭킹 조회
    public async void GetTopRankings()
    {
        List<RankingEntry> topRankings = await RealtimeDatabase.GetTopRankings();
        foreach (var entry in topRankings)
        {
            /*var gtObject = Instantiate(goatItem, parentTransform);
            gtObject.SetGoatData(entry,ClickScore);*/
            
            Debug.Log($"{entry.rank}위 - 모델 이름 : {entry.modelName}, 제작자 이름 : {entry.username}, 좋아요 수 : {entry.score}");
        }
    }

    // 특정 모델의 랭킹 조회
    public async void GetModelRank(string modelId)
    {
        RankingEntry modelRank = await RealtimeDatabase.GetModelRank(modelId);
        if (modelRank != null)
        {
            Debug.Log($"{modelRank.modelName}'s rank: {modelRank.rank} (Score: {modelRank.score})");
        }
    }
}