using System;
using UnityEngine;

[Serializable]
public class LandObject
{
    public string key;
    public int objectIndex;
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public SerializableVector3 scale;

    public LandObject() { }
    public LandObject(string key, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.key = key;
        this.position = new SerializableVector3(position);
        this.rotation = new SerializableVector3(rotation);
        this.scale = new SerializableVector3(scale);
    }

    public LandObject(string key, int objectIndex, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.key = key;
        this.objectIndex = objectIndex;
        this.position = new SerializableVector3(position);
        this.rotation = new SerializableVector3(rotation);
        this.scale = new SerializableVector3(scale);
    }
}

public static class LandObjectConverter
{
    public static LandObject ConvertToLandObject(GameObject gameObject, int selectedObjectIndex)
    {
        if (gameObject == null)
        {
            Debug.LogError("The provided GameObject is null.");
            return null;
        }
        
        // Generate a unique key or fallback to a default key
        string key = RealtimeDatabase.GenerateKey() ?? $"default_{Guid.NewGuid()}";
        
        // LandObject instance creation
        return new LandObject(
            key,
            selectedObjectIndex,
            gameObject.transform.position,
            gameObject.transform.eulerAngles,
            gameObject.transform.localScale
        );
    }
    
    public static LandObject ConvertToModelObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Debug.LogError("The provided GameObject is null.");
            return null;
        }
        
        // Generate a unique key or fallback to a default key
        string key = RealtimeDatabase.GenerateKey() ?? $"default_{Guid.NewGuid()}";
        
        // LandObject instance creation
        return new LandObject(
            key,
            gameObject.transform.position,
            gameObject.transform.eulerAngles,
            gameObject.transform.localScale
        );
    }
}