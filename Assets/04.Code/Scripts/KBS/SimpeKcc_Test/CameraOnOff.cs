using Fusion;
using UnityEngine;

public class CameraOnOff : NetworkBehaviour
{
    public Camera FaceCamera;
    
    [SerializeField]
    private Camera PlayerCamera;
    
    public bool toggleFaceCam;

    public Canvas CaptureCanvas;
    
    
    
    void Start()
    {
        if (!HasStateAuthority)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            toggleFaceCam = !toggleFaceCam;
            SelfToPlayerCamera();
            
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

    public void SelfToPlayerCamera()
    {
        toggleFaceCam = !toggleFaceCam;
        
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
