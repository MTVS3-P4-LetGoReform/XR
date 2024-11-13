using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CameraOverlayController : MonoBehaviour
{
    public Camera baseCamera;       // Base 카메라
    private Camera overlayCamera;
    private GameObject newOverlayCamera;

    private Coroutine currentCoroutine;

    private bool isCameraOn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCoroutine = StartCoroutine(FindCamera());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    private IEnumerator FindCamera()
    {
        newOverlayCamera = null;
        
        while (newOverlayCamera == null)
        {
            newOverlayCamera = GameObject.FindWithTag("UICamera");

            if (newOverlayCamera == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                overlayCamera = newOverlayCamera.GetComponent<Camera>();
        
                var baseCameraData = baseCamera.GetUniversalAdditionalCameraData();
        
                var overlayCameraData = overlayCamera.GetUniversalAdditionalCameraData();
                overlayCameraData.renderType = CameraRenderType.Overlay;
            
                // Base 카메라의 스택에 Overlay 카메라를 추가합니다.
                baseCameraData.cameraStack.Add(overlayCamera);

                isCameraOn = true;
            }
            
        }
        
        
        
    }
}
