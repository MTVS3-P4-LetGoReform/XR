using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public string userId;
    public string userName;

    public List<Friend> friends;
    public EstateInfo estateInfo;

    public User()
    {
        
    }

    public User(string userId, string username)
    {
        this.userId = userId;
        this.userName = username;
    }
    
    public User(string userId, string username, List<Friend> friends)
    {
        this.userId = userId;
        this.userName = username;
        this.friends = friends;
    }
}
