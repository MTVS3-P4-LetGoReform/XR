using System;

[Serializable]
public class Friend
{
    public string friendName;         // 친구 이름
    public string friendEmail;        // 친구 이메일
    public string friendProfileImage; // 친구 프로필 이미지 URL
    public long addedOn;              // 친구 추가 시간 (Unix Timestamp)

    public Friend() { }

    public Friend(string friendName, string friendEmail, string friendProfileImage, long addedOn)
    {
        this.friendName = friendName;
        this.friendEmail = friendEmail;
        this.friendProfileImage = friendProfileImage;
        this.addedOn = addedOn;
    }
}