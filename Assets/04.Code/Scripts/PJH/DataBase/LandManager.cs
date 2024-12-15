using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class LandManager : MonoBehaviour
{
    [SerializeField] private LandUIController uiController;
    [SerializeField] private LandObjectController objectController;
    private string _userId;
    private string _userName;

    private async void Start()
    {
        await InitializeLandAsync();
        await InitializeLandInfoAsync(_userId, _userName);
        StartListening();
    }

    private async UniTask InitializeLandAsync()
    {
        try
        {
            var properties = RunnerManager.Instance.runner.SessionInfo.Properties;
            if (properties.TryGetValue("UserId", out var sessionProperty))
            {
                _userId = sessionProperty;
            }

            _userName = await RealtimeDatabase.FindNameByIdAsync(_userId);
            Debug.Log($"DB에서 정보를 불러옵니다. 현재 영지: {_userId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"영지 초기화 실패: {e.Message}");
        }
    }

    private async UniTask InitializeLandInfoAsync(string userId, string userName)
    {
        try 
        {
            var landInfo = await RealtimeDatabase.ReadDataAsync<LandInfo>($"user_land/{userId}/landInfo");
            
            if (landInfo == null)
            {
                Debug.Log("LandInfo 초기화 중...");
                var newLandInfo = new LandInfo(userName);
                
                await RealtimeDatabase.CreateDataAsync($"user_land/{userId}/landInfo", newLandInfo);
                Debug.Log("새로운 LandInfo 생성 완료");
                
                uiController.UpdateLandInfo(newLandInfo,userId);
            }
            else
            {
                uiController.UpdateLandInfo(landInfo,userId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"LandInfo 초기화 실패: {e.Message}");
        }
    }

    private void StartListening()
    {
        // LandInfo 변경 구독
        RealtimeDatabase.ListenForDataChanges<LandInfo>(
            $"user_land/{_userId}/landInfo",
            landInfo => uiController.UpdateLandInfo(landInfo,_userId),
            exception => Debug.LogError($"LandInfo 수신 오류: {exception.Message}")
        );

        // 오브젝트 변경 구독
        RealtimeDatabase.ListenForDataChanges<List<LandObject>>(
            $"user_land/{_userId}/objects",
            objects => objectController.UpdateObjects(objects),
            exception => Debug.LogError($"Objects 수신 오류: {exception.Message}")
        );
    }
    
    private void OnDestroy()
    {
        // 씬 전환 시 리스너 정리
        RealtimeDatabase.StopListeningForDataChanges($"user_land/{_userId}/landInfo");
        RealtimeDatabase.StopListeningForDataChanges($"user_land/{_userId}/objects");
    }
}
