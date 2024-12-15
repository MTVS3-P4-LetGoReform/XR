using Newtonsoft.Json;

public class LandInfo
{
    public string UserName { get; set; }
    public int Visitors { get; set; }
    public int Likes { get; set; }

    // 기본 생성자 추가
    public LandInfo()
    {
    }

    // 또는 JsonConstructor 특성을 사용한 생성자
    [JsonConstructor]
    public LandInfo(string userName)
    {
        UserName = userName;
        Visitors = 0;
        Likes = 0;
    }
}