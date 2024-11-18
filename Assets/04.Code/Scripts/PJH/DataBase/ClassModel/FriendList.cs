using System;
using System.Collections.Generic;

[Serializable]
public class FriendList
{
    public Dictionary<string, Friend> friends = new Dictionary<string, Friend>();

    public FriendList() { }

    public void AddFriend(Friend friend)
    {
        friends[friend.userID] = friend;
    }
}