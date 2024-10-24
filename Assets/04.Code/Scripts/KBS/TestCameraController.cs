using UnityEngine;

public class TestCameraController : MonoBehaviour
{
    public Vector2 CameraSpeed = new Vector2(100, 100);
    
    private float Pitch = 0;
    private float Yaw = 0;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Pitch += Input.GetAxis("Mouse Y") * CameraSpeed.y * Time.deltaTime;
        Yaw += Input.GetAxis("Mouse X") * CameraSpeed.x * Time.deltaTime;

        Camera.main.transform.eulerAngles = new Vector3(Pitch, Yaw, 0);
    }
}
