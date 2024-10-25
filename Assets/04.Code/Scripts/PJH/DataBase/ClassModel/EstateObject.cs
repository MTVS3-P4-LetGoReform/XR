using UnityEngine;
using System;

[Serializable]
public class EstateObject
{
    public string objectId;
    public string objectType;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public EstateObject() {}

    public EstateObject(string objectId, string objectType, Transform transform)
    {
        this.objectId = objectId;
        this.objectType = objectType;
        this.position = transform.position;
        this.rotation = transform.eulerAngles;
        this.scale = transform.localScale;
    }

    public EstateObject(string objectId, Transform transform)
    {
        this.objectId = objectId;
        this.objectType = "Unknown";
        this.position = transform.position;
        this.rotation = transform.eulerAngles;
        this.scale = transform.localScale;
    }

    public void ApplyToTransform(Transform targetTransform)
    {
        Debug.Log($"Applying to Transform:\nPosition: {this.position}\nRotation: {this.rotation}\nScale: {this.scale}");
        
        targetTransform.position = this.position;
        targetTransform.eulerAngles = this.rotation;
        targetTransform.localScale = this.scale;
    }
}