using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LandUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text landMakerName;
    [SerializeField] private TMP_Text visitors;
    [SerializeField] private TMP_Text likes;
    [SerializeField] private Button likeButton;

    private string _lastLikeUserKey;
    private string _lastVisitUserKey;
    private LandInfo _currentLandInfo;

    private void Start()
    {
        likeButton.onClick.AddListener(HandleLikeButtonClick);
    }
    
    private async UniTaskVoid CheckVisitorCount()
    {
        try 
        {
            if (_currentLandInfo == null) return;
        
            _lastVisitUserKey = $"LastVisit{_currentLandInfo.UserName}_{UserData.Instance.UserId}";
        
            if (CanVisitToday(_lastVisitUserKey))
            {
                _currentLandInfo.Visitors++;
                SaveVisitDate();
                await UpdateLandInfoAsync();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"방문자 카운트 업데이트 실패: {e.Message}");
        }
    }
    
    private bool CanVisitToday(string key)
    {
        string lastDate = PlayerPrefs.GetString(key, "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        return string.IsNullOrEmpty(lastDate) || lastDate != today;
    }
    
    private void SaveVisitDate()
    {
        PlayerPrefs.SetString(_lastVisitUserKey, DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
    }


    public void UpdateLandInfo(LandInfo landInfo)
    {
        if (landInfo == null) return;
        
        _currentLandInfo = landInfo;
        UpdateUI();
        CheckVisitorCount().Forget(); // LandInfo가 업데이트될 때마다 방문자 체크
    }

    private void UpdateUI()
    {
        landMakerName.text = _currentLandInfo.UserName;
        visitors.text = $"방문자 수: {_currentLandInfo.Visitors}";
        likes.text = $"좋아요 수: {_currentLandInfo.Likes}";
    }

    private async void HandleLikeButtonClick()
    {
        _lastLikeUserKey = $"LastLike{_currentLandInfo.UserName}";
        if (!CanLikeToday(_lastLikeUserKey)) return;

        _currentLandInfo.Likes++;
        SaveLikeDate();
        await UpdateLandInfoAsync();
    }

    private bool CanLikeToday(string key)
    {
        string lastDate = PlayerPrefs.GetString(key, "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        return string.IsNullOrEmpty(lastDate) || lastDate != today;
    }

    private void SaveLikeDate()
    {
        PlayerPrefs.SetString(_lastLikeUserKey, DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
    }

    private async UniTask UpdateLandInfoAsync()
    {
        try
        {
            await RealtimeDatabase.SetUserLandInfoAsync(UserData.Instance.UserId, _currentLandInfo);
        }
        catch (Exception e)
        {
            Debug.LogError($"LandInfo 업데이트 실패: {e.Message}");
        }
    }
}
