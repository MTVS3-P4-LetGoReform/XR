using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionController : MonoBehaviour
{
    public Camera playerCamera;

    public string invisibleLayerName = "Invisible";
    
    public List<string> exceptionLayerNames = new List<string>();
    
    private List<int> exceptionLayers = new List<int>();

    private int originalLayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (string layerName in exceptionLayerNames)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1) // 유효한 레이어만 추가
            {
                exceptionLayers.Add(layer);
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' is not a valid layer.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int otherLayer = other.gameObject.layer;
        
        if (exceptionLayers.Contains(otherLayer))
        {
            Debug.Log($"{other}는 예외 레이어에 속하므로 무시합니다.");
            return;
        }
        
        originalLayer = other.gameObject.layer;

        other.gameObject.layer = LayerMask.NameToLayer(invisibleLayerName);
        Debug.Log($"{other}에 닿았다.");
    }


    private void OnTriggerExit(Collider other)
    {
        other.gameObject.layer = originalLayer;
    }
}
