using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    public Button join;
    public StatusImage image;


    [Header("Popup")]
    public GameObject friendPopUpPrefab;
    public Transform popUpParent;
    
    public async void SetFriendData(User friend , Transform parent)
    {
        if (friend == null)
        {
            Debug.LogError("[FriendItem] - friend값이 null입니다.");
            return;
        }
        
        Debug.Log("[FriendItem] - friendName : " + friend.name );
        friendNameText.text = friend.name;
        statusImage.sprite = friend.onlineStatus ? image.onlineImage : image.offlineImage;
        popUpParent = parent;
        
        await CreatePopUp();
        Debug.Log("[FriendItem] - PopUp생성 완료");
    }

    private async UniTask CreatePopUp()
    {
        var popUpItem = Instantiate(friendPopUpPrefab, popUpParent);
        var friendPopUp = popUpItem.GetComponent<FriendPopUpInfo>();
        await UniTask.Yield();
        
        friendPopUp.popUpText.text = friendNameText.text + "님의 테마파크로 이동하시겠습니까?";
        friendPopUp.yes.onClick.AddListener(() => GotoPersonal(FriendId));
        join.onClick.AddListener(()=> popUpItem.SetActive(true));
    }
    
    private async void GotoPersonal(string userId)
    {
        string input = userId;
        string result = input.Substring(6); // "users/"가 6글자이므로 6번째 인덱스부터 끝까지 추출
        
        Debug.Log("[FriendItem] - UserId: "+ result);
        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = result,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "UserId", result },
            },
        };
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(args,3);
    }
}
