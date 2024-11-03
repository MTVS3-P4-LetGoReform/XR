using System;
using UnityEngine;

[Serializable]
public class LandObject
{
    public string type;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public LandObject() { }

    public LandObject(string type, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.type = type;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}