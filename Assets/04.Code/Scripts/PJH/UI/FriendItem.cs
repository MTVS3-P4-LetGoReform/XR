using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    public Text friendNameText;
    public Text onlineStatusText;

    public void SetFriendData(User friend)
    {
        friendNameText.text = friend.name;
        onlineStatusText.text = friend.onlineStatus ? "Online" : "Offline";
        onlineStatusText.color = friend.onlineStatus ? Color.green : Color.gray;
    }
}