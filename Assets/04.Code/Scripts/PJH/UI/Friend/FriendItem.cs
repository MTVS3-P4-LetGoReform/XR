using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    //public TMP_Text onlineStatusText;
    [Header("PlayerInfo")]
    public TMP_Text friendNameText;
    public Image statusImage;

    public string FriendId { get; set; }

    [Serializable]
    public class StatusImage
    {
        public Sprite onlineImage;
        public Sprite offlineImage;
    }
    
    [Header("Object")]
    public Button yes;
    public StatusImage image;


    [Header("Popup")] 
    public TMP_Text popupText;
    
    public void SetFriendData(User friend)
    {
        if (friend == null)
        {
            Debug.LogError("[FriendItem] - friend값이 null입니다.");
            return;
        }
        
        friendNameText.text = friend.name;
        statusImage.sprite = friend.onlineStatus ? image.onlineImage : image.offlineImage;
        popupText.text = friend.name + "님의 테마파크로 이동하시겠습니까?";
        yes.onClick.AddListener(() => GotoPersonal(FriendId));
    }
    
    public async void GotoPersonal(string userId)
    {
        Debug.Log("userid: "+ userId);
        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            //Scene = SceneRef.FromIndex(2),
            SessionName = userId,
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                {"UserId", userId},
            }
        };
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(args,3);
    }
}
