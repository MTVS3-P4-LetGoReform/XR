using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public Transform TpcameraPoint;
    public Transform FpCameraPoint;

    public float rotSpeed = 200f;

    private bool togglePov = false;
    private float mx = 0f;
    private float my = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
       // transform.position = TpcameraPoint.transform.position;
        CameraRotate();
        ChangeCamPosition();
        
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
             togglePov = true;
             if (togglePov)
             {
                 transform.position = FpCameraPoint.transform.position;
             }

             togglePov = !togglePov; 
        }
        else
        {
             transform.position = TpcameraPoint.transform.position;
        } 
    }
}
