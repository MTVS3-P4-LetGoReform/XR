using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserLand
{
    public List<LandObject> objects = new List<LandObject>();

    public UserLand()
    {
        objects = new List<LandObject>();
    }

    public void AddObject(LandObject landObject)
    {
        objects.Add(landObject);
    }
}