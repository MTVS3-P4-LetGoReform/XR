using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class RawImageMaker : MonoBehaviour
{
    public Camera captureCamera;
    public RenderTexture renderTexture;
    public List<GameObject> prefabs;
    public List<Image> uiImage;
    
    void Start()
    { 
        StartCoroutine(Capture());
    }

    private IEnumerator Capture()
    {
        for(int i = 0; i < prefabs.Count; i++)
        {
            GameObject instance = Instantiate(prefabs[i]);
            instance.transform.position = captureCamera.transform.position + captureCamera.transform.forward * 5;

            uiImage[i].material = new Material(Shader.Find("UI/Default"));
            uiImage[i].material.mainTexture = renderTexture;

            yield return new WaitForEndOfFrame();
            
            Destroy(instance);
            
            
            
        }
    }
}
