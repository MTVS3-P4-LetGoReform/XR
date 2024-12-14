using UnityEngine;

public class CameraOnOff : MonoBehaviour
{
    public Camera FaceCamera;
    public Camera PlayerCamera;
    
    public bool toggleFaceCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            toggleFaceCam = !toggleFaceCam;
        }

        if (toggleFaceCam)
        {
            FaceCamera.gameObject.SetActive(true);
            PlayerCamera.gameObject.SetActive(false);
        }
        else
        {
            FaceCamera.gameObject.SetActive(false);
            PlayerCamera.gameObject.SetActive(true);
        }
    }
}
