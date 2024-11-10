using System;

[Serializable]
public class User
{
    public string name;
    public string email;
    public string profileImage;
    public bool onlineStatus;
    public long lastLogin;

    public User() { }

    public User(string name, string email, string profileImage, bool onlineStatus, long lastLogin)
    {
        this.name = name;
        this.email = email;
        this.profileImage = profileImage;
        this.onlineStatus = onlineStatus;
        this.lastLogin = lastLogin;
    }
}