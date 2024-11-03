using System;
using UnityEngine;

[Serializable]
public class LandObject
{
    public string type;
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public SerializableVector3 scale;

    public LandObject() { }

    public LandObject(string type, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.type = type;
        this.position = new SerializableVector3(position);
        this.rotation = new SerializableVector3(rotation);
        this.scale = new SerializableVector3(scale);
    }
}
