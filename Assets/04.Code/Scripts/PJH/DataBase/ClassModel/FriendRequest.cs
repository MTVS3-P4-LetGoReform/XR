using System;

[Serializable]
public class FriendRequest
{
    public string requesterName;      // 요청 보낸 사람 이름
    public string requesterEmail;     // 요청 보낸 사람 이메일
    public long requestedOn;          // 요청 보낸 시간 (Unix Timestamp)

    public FriendRequest() { }

    public FriendRequest(string requesterName, string requesterEmail, long requestedOn)
    {
        this.requesterName = requesterName;
        this.requesterEmail = requesterEmail;
        this.requestedOn = requestedOn;
    }
}