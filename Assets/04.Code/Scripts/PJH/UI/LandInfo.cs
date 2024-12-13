
public class LandInfo
{
    public string UserName;
    public int Visitors;
    public int Likes;

    public LandInfo(string userName)
    {
        UserName = userName;
        Visitors = 0;
        Likes = 0;
    }
    
    public LandInfo(string userName, int visitors, int likes)
    {
        UserName = userName;
        Visitors = visitors;
        Likes = likes;
    }

    public void Set(string userName, int visitors, int likes)
    {
        UserName = userName;
        Visitors = visitors;
        Likes = likes;
    }
}
