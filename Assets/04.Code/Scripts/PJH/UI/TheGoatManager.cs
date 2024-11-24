using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TheGoatManager : MonoBehaviour
{
    public WebApiData webApiData;
    public DebugModeData debugModeData;
    private StorageDatabase _storageDatabase;
    
    public GoatItem topItemPrefab;
    public GoatItem secItemPrefab;
    
    public Transform topParent;
    public Transform secParent;

    private async void Start()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        FirebaseDatabase.DefaultInstance.GoOffline();
        FirebaseDatabase.DefaultInstance.GoOnline();
        
        _storageDatabase = new StorageDatabase(webApiData, debugModeData);
        await RealtimeDatabase.InitializeFirebaseAsync();
        
        GetTopRankings().Forget();
    }
    
    // 점수 업데이트
    public async void ClickScore(string modelId)
    {
        var userRank = await RealtimeDatabase.GetModelRank(modelId);
        Debug.Log("기존 좋아요 수 : " + userRank.score);
        await RealtimeDatabase.UpdateScore(modelId, userRank.score);
        GetTopRankings().Forget();
    }

    // 상위 랭킹 조회
    public async UniTask GetTopRankings()
    {
        List<RankingEntry> topRankings = await RealtimeDatabase.GetTopRankings();
        foreach (var entry in topRankings)
        {
            if (entry.rank == 1)
            {
                var gtObject = Instantiate(topItemPrefab, topParent);
                gtObject.SetGoatData(entry,ClickScore);
                SetImage(entry.selectImageName,gtObject.modelImage);
            }
            else
            {
                var gtObject = Instantiate(secItemPrefab, secParent);
                gtObject.SetGoatData(entry,ClickScore);
                SetImage(entry.selectImageName,gtObject.modelImage);
            }
            
            Debug.Log($"{entry.rank}위 - 모델 이름 : {entry.modelName}, 제작자 이름 : {entry.username}, 좋아요 수 : {entry.score}");
        }
    }
    
    private async void SetImage(string imageName, Image modelImage)
    {
        try
        {
            if (string.IsNullOrEmpty(imageName))
            {
                imageName = "DebugModeImage.png";
            }
            string url  = Path.Combine(Application.persistentDataPath,"Images",imageName);
            await _storageDatabase.DownLoadImage(imageName,url);
            UpdateImage(url,modelImage).Forget();
        }
        catch (Exception e)
        {
            Debug.LogWarning("이미지 설정에 실패했습니다. : " + e.Message);
            //throw new Exception();
        }
    } 
    
    private async UniTaskVoid UpdateImage(string url, Image targetImage)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("targetImage가 null입니다. 스프라이트를 설정할 수 없습니다.");
            return;
        }
        
        if (!File.Exists(url))
        {
            Debug.LogWarning("경로에 이미지 파일이 존재하지 않습니다.");
            return;
        }

        var req = UnityWebRequestTexture.GetTexture(url);
        await req.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(req);
        
        var sprite = Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        sprite.name = Path.GetFileName(url);
        targetImage.sprite = sprite;

        Debug.Log($"스프라이트 설정 완료: {sprite.name}");
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