using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraOverlayController : MonoBehaviour
{
    public Camera baseCamera;       // Base 카메라
    private Camera overlayCamera;
    private GameObject newOverlayCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(FindCamera());
    }
    
    
    private IEnumerator FindCamera()
    {
        GameObject cameraObject = null;

        while (cameraObject == null)
        {
            cameraObject = GameObject.FindWithTag("UICamera");

            if (cameraObject == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                overlayCamera = cameraObject.GetComponent<Camera>();
        
                var baseCameraData = baseCamera.GetUniversalAdditionalCameraData();
        
                var overlayCameraData = overlayCamera.GetUniversalAdditionalCameraData();
                overlayCameraData.renderType = CameraRenderType.Overlay;
            
                // Base 카메라의 스택에 Overlay 카메라를 추가합니다.
                baseCameraData.cameraStack.Add(overlayCamera);
            }
        }
        
    }
}
