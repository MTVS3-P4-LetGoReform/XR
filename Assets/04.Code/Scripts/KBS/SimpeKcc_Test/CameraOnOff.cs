using Fusion;
using UnityEngine;

public class CameraOnOff : NetworkBehaviour
{
    public Camera FaceCamera;
    public Camera PlayerCamera;
    
    public bool toggleFaceCam;

    public Canvas CaptureCanvas;
    
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            toggleFaceCam = !toggleFaceCam;
            
        }

        if (toggleFaceCam)
        {
            FaceCamera.gameObject.SetActive(true);
            PlayerCamera.gameObject.SetActive(false);
            CaptureCanvas.gameObject.SetActive(true);
        }
        else
        {
            FaceCamera.gameObject.SetActive(false);
            PlayerCamera.gameObject.SetActive(true);
            CaptureCanvas.gameObject.SetActive(false);
        }
    }
}
