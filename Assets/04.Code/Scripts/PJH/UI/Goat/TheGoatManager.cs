using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TheGoatManager : MonoBehaviour
{
    [Header("Configuration")]
    public WebApiData webApiData;
    public DebugModeData debugModeData;
    
    [Header("UI Prefabs")]
    public GoatItem topItemPrefab;
    public GoatItem secItemPrefab;
    
    [Header("UI Parents")]
    public Transform topParent;
    public Transform secParent;
    
    private async void Start()
    {
        await InitializeDatabasesAsync();
        await GetTopRankings();
    }

    /// <summary>
    /// Firebase 및 Storage 데이터베이스 초기화
    /// </summary>
    private async UniTask InitializeDatabasesAsync()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        FirebaseDatabase.DefaultInstance.GoOffline();
        FirebaseDatabase.DefaultInstance.GoOnline();
        
        StorageDatabase.InitializStorageDatabase(webApiData, debugModeData);
        await RealtimeDatabase.InitializeFirebaseAsync();
    }
    
    /// <summary>
    /// 모델의 점수를 업데이트하고 랭킹을 새로고침합니다.
    /// </summary>
    /// <param name="modelId">업데이트할 모델의 ID</param>
    public async void ClickScore(string modelId)
    {
        try
        {
            var userRank = await RealtimeDatabase.GetModelRank(modelId);
            Debug.Log($"기존 좋아요 수: {userRank.score}");
            await RealtimeDatabase.UpdateScore(modelId, userRank.score);
            await GetTopRankings();
        }
        catch (Exception e)
        {
            Debug.LogError($"점수 업데이트 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 상위 랭킹을 조회하고 UI를 업데이트합니다.
    /// </summary>
    private async UniTask GetTopRankings()
    {
        ClearRankingUI();
        
        try
        {
            var topRankings = await RealtimeDatabase.GetTopRankings();
            await CreateRankingItems(topRankings);
        }
        catch (Exception e)
        {
            Debug.LogError($"랭킹 조회 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 랭킹 UI를 초기화합니다.
    /// </summary>
    private void ClearRankingUI()
    {
        foreach (Transform child in topParent)
            Destroy(child.gameObject);
        foreach (Transform child in secParent)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// 랭킹 항목을 생성하고 UI에 추가합니다.
    /// </summary>
    /// <param name="rankings">랭킹 목록</param>
    private async UniTask CreateRankingItems(List<RankingEntry> rankings)
    {
        foreach (var entry in rankings)
        {
            GoatItem gtObject;
            if (entry.rank == 1)
                gtObject = Instantiate(topItemPrefab, topParent);
            else
                gtObject = Instantiate(secItemPrefab, secParent);

            gtObject.SetGoatData(entry, ClickScore);
            await SetImage(entry.selectImageName, gtObject.modelImage);
            
            Debug.Log($"{entry.rank}위 - 모델 이름: {entry.modelName}, 제작자 이름: {entry.username}, 좋아요 수: {entry.score}");
        }
    }
    
    /// <summary>
    /// 이미지를 다운로드하고 UI에 설정합니다.
    /// </summary>
    /// <param name="imageName">이미지 파일 이름</param>
    /// <param name="modelImage">설정할 Image 컴포넌트</param>
    private async UniTask SetImage(string imageName, Image modelImage)
    {
        try
        {
            imageName = string.IsNullOrEmpty(imageName) ? "DebugModeImage.png" : imageName;
            string url = Path.Combine(Application.persistentDataPath, "Images", imageName);
            await StorageDatabase.RankingImageDownLoad(imageName, url);
            await UpdateImage(url, modelImage);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"이미지 설정 실패: {e.Message}");
        }
    } 
    
    /// <summary>
    /// 이미지를 로드하고 UI에 적용합니다.
    /// </summary>
    /// <param name="url">이미지 파일 경로</param>
    /// <param name="targetImage">설정할 Image 컴포넌트</param>
    private async UniTask UpdateImage(string url, Image targetImage)
    {
        if (targetImage == null || !File.Exists(url))
        {
            Debug.LogWarning("이미지 업데이트 실패: 유효하지 않은 Image 컴포넌트 또는 파일 경로");
            return;
        }

        try
        {
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
        catch (Exception e)
        {
            Debug.LogError($"이미지 로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 특정 모델의 랭킹을 조회합니다.
    /// </summary>
    /// <param name="modelId">조회할 모델의 ID</param>
    public async void GetModelRank(string modelId)
    {
        try
        {
            RankingEntry modelRank = await RealtimeDatabase.GetModelRank(modelId);
            if (modelRank != null)
            {
                Debug.Log($"{modelRank.modelName}의 랭킹: {modelRank.rank} (점수: {modelRank.score})");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"모델 랭킹 조회 실패: {e.Message}");
        }
    }
}
