using System;
using UnityEngine;

public class CameraCollisionController : MonoBehaviour
{
    public Camera playerCamera;

    public string invisibleLayerName = "Invisible";

    private int originalLayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        originalLayer = other.gameObject.layer;

        other.gameObject.layer = LayerMask.NameToLayer(invisibleLayerName);
        Debug.Log($"{other}에 닿았다.");
    }


    private void OnTriggerExit(Collider other)
    {
        other.gameObject.layer = originalLayer;
    }
}
