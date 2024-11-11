using System;
using UnityEngine;
using UnityEngine.UI;

public class RewordCanvas : MonoBehaviour
{
    public Canvas masterCanvas;
    public Button masterRewordButton;
    
    public Canvas userCanvas;
    public Button userRewordButton;

    [SerializeField]
    private WebApiData webApiData;

    private void Start()
    {
        masterRewordButton.onClick.AddListener(MasterReword);
        userRewordButton.onClick.AddListener(UserReword);
    }

    private async void MasterReword()
    {
        var userId = UserData.Instance.UserId;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        var properties = RunnerManager.Instance.runner.SessionInfo.Properties;
        
        string modelId = "";
        if (properties.TryGetValue("ModelId", out var sessionProperty))
        {
            modelId = sessionProperty;
            Debug.Log("결괏값: " + modelId);
        }
        else
        {
            Debug.LogWarning($"{properties}: 이미지를 불러오지 못했습니다.");
        }

        RealtimeDatabase.CopyModelToUser(userId, modelId);
        Debug.Log("보상획득: 스태츄");
        await RunnerManager.Instance.JoinPublicSession();
    }
    
    private async void UserReword()
    {
        Debug.Log("보상획득: 크레딧");
        Cursor.lockState = CursorLockMode.Locked;
        //크레딧 얻는 로직 추가 필요
        await RunnerManager.Instance.JoinPublicSession();
    }
}
