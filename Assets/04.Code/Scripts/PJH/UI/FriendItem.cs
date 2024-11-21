using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    public TMP_Text friendNameText;
    public TMP_Text onlineStatusText;

    public Image statusImage;
    public Sprite onlineImage;
    public Sprite offlineImage;

    public void SetFriendData(User friend)
    {
        friendNameText.text = friend.name;
        onlineStatusText.text = friend.onlineStatus ? "Online" : "Offline";
        onlineStatusText.color = friend.onlineStatus ? Color.green : Color.gray;
    }
}