using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class EstateInfo
{
    public string EstateName;
    public List<EstateObject> objects;

    public EstateInfo()
    {
        
    }
    
    public EstateInfo(string estateName, List<EstateObject> objectsInfo)
    {
        EstateName = estateName;
        objects = objectsInfo;
    }

    public EstateInfo(List<EstateObject> objects)
    {
        this.objects = objects;
    }
}
