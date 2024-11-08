using Unity.Cinemachine;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public Transform TpcameraPoint;
    public Transform FpCameraPoint;
    public CinemachineCamera placementCamera;
    private Camera camera;

    public float rotSpeed = 200f;

    private bool togglePov = false;
    private bool turnCam = false;
    private float mx = 0f;
    private float my = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
       // transform.position = TpcameraPoint.transform.position;
        CameraRotate();
        ChangeCamPosition();
        CameraTurnOnOff();
    }

    private void CameraRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        mx += mouseX * rotSpeed * Time.deltaTime;
        my += mouseY * rotSpeed * Time.deltaTime;
        my = Mathf.Clamp(my, -90f, 90f);
        transform.eulerAngles = new Vector3(-my, mx, 0f);
    }

    private void ChangeCamPosition()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
             togglePov = !togglePov; 
        }
        if (togglePov)
        {
            transform.position = FpCameraPoint.transform.position;
        }
        else
        {
             transform.position = TpcameraPoint.transform.position;
        } 
    }

    private void CameraTurnOnOff()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            turnCam = !turnCam;
        }

        if (turnCam)
        {
            //camera.gameObject.SetActive(false);
            placementCamera.gameObject.SetActive(true);
        }
        else
        {
            //camera.gameObject.SetActive(true);
            placementCamera.gameObject.SetActive(false);
        }
    }
}
