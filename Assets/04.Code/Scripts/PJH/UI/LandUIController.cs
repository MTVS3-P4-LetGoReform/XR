using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandUIController : MonoBehaviour
{
    public event Action UpdateInfo; 
    public LandInfo LandInfo;
    
    private string _userId;
    
    public TMP_Text landMakerName;
    public TMP_Text visitors;
    public TMP_Text likes;

    public Button like;
    public Button addFriend;
    
    private string _lastAttendanceUserKey;
    private string _lastLikeUserKey;
    

    private void Start()
    {
        RunnerManager.Instance.IsSpawned += AfterSpawned;
        like.onClick.AddListener(CheckLikes);
    }

    private void AfterSpawned()
    {
        UpdateInfo += UpdateLandInfo;
    }
    
    private void UpdateLandInfo()
    {
        if (_userId == null)
        {
            Debug.Log("_userId값이 없습니다.");
        }
        
        RealtimeDatabase.SetUserLandInfo(_userId,LandInfo);
    }
    
    public void SetInfo(string userId, string userName, LandInfo info)
    {
        LandInfo = info;
        
        CheckAttendance();
        
        landMakerName.text = LandInfo.UserName;
        visitors.text = $"방문자 수: {LandInfo.Visitors}";
        likes.text = $"좋아요 수: {LandInfo.Likes}";

        RealtimeDatabase.SetUserLandInfo(userId, LandInfo);
        
        Debug.Log("LandUIController - SetInfo = 이름 : " + LandInfo.UserName);
        Debug.Log("LandUIController - SetInfo = 방문자 수 : " + LandInfo.Visitors);
        Debug.Log("LandUIController - SetInfo = 좋아요 수 : " + LandInfo.Likes);
    }

    private void CheckLikes()
    {
        _lastLikeUserKey = "LastLike" + LandInfo.UserName;
        
        if (!CanCheckKey(_lastLikeUserKey))
            return;
        
        // 좋아요 횟수 증가
        LandInfo.Likes++;
        
        // 현재 날짜 저장
        PlayerPrefs.SetString(_lastLikeUserKey, DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
        
        UpdateInfo?.Invoke();
    }
    
    private void CheckAttendance()
    {
        _lastAttendanceUserKey = "LastAttendance" + LandInfo.UserName;
        
        if (!CanCheckKey(_lastAttendanceUserKey)) 
            return;

        // 출석 횟수 증가
        LandInfo.Visitors++;
        
        // 현재 날짜 저장
        PlayerPrefs.SetString(_lastAttendanceUserKey, DateTime.Now.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
        
        UpdateInfo?.Invoke();
    }
    
    private bool CanCheckKey(string key)
    {
        string lastDate = PlayerPrefs.GetString(key, "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        
        return string.IsNullOrEmpty(lastDate) || lastDate != today;
    }

    
}
