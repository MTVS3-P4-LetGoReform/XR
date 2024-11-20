using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public string name;
    public string email;
    public string profileImage;
    public bool onlineStatus;
    public long lastLogin;
    public Dictionary<string, bool> friends;         // 친구 ID 목록
    public Dictionary<string, bool> incomingRequests; // 받은 친구 요청 목록
    public Dictionary<string, bool> outgoingRequests; // 보낸 친구 요청 목록
    
    public User()
    {
        friends = new Dictionary<string, bool>();
        incomingRequests = new Dictionary<string, bool>();
        outgoingRequests = new Dictionary<string, bool>();
    }
    
    public User(string name, string email, string profileImage, bool onlineStatus, long lastLogin)
    {
        this.name = name;
        this.email = email;
        this.profileImage = profileImage;
        this.onlineStatus = onlineStatus;
        this.lastLogin = lastLogin;
        friends = new Dictionary<string, bool>();
        incomingRequests = new Dictionary<string, bool>();
        outgoingRequests = new Dictionary<string, bool>();
    }
}