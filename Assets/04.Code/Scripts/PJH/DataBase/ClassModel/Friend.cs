using System;

[Serializable]
public class Friend
{
    public string userID;
    public string name;
    public bool onlineStatus;

    public Friend() { }

    public Friend(string userID, string name, bool onlineStatus)
    {
        this.userID = userID;
        this.name = name;
        this.onlineStatus = onlineStatus;
    }
}