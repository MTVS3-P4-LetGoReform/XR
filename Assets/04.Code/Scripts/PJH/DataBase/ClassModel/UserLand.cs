using System;
using System.Collections.Generic;

[Serializable]
public class UserLand
{
    public List<LandObject> objects = new List<LandObject>();
    public LandInfo landInfo;
    
    public UserLand(string userName)
    {
        landInfo = new LandInfo(userName);
        objects = new List<LandObject>();
    }

    public void AddObject(LandObject landObject)
    {
        objects.Add(landObject);
    }
}