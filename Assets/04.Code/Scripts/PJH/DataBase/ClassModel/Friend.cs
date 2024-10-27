using System;
using System.Collections.Generic;

[Serializable]
public class Friend
{
    public string friendId;
    public string friendName;

    public Friend()
    {
        
    }

    public Friend(string friendId,string friendName)
    {
        this.friendId = friendId;
        this.friendName = friendName;
    }

    public Dictionary<string, Object> ToDictionary()
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();
        result["friendId"] = friendId;
        result["friendName"] = friendName;

        return result;
    }
}