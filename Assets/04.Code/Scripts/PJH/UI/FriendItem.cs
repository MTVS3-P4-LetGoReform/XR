using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    //public TMP_Text onlineStatusText;
    public TMP_Text friendNameText;
    public Button yes;
    public Image statusImage;
    public Sprite onlineImage;
    public Sprite offlineImage;

    public string friendId;
    
    public void SetFriendData(User friend)
    {
        friendNameText.text = friend.name;
        statusImage.sprite = friend.onlineStatus ? onlineImage : offlineImage;
        /*onlineStatusText.text = friend.onlineStatus ? "Online" : "Offline";
        onlineStatusText.color = friend.onlineStatus ? Color.green : Color.gray;*/
        yes.onClick.AddListener(() => GotoPersonal(friendId));
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
