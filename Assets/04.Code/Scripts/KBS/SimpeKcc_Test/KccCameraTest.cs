
using Fusion;
using UnityEngine;

public class KccCameraTest : MonoBehaviour
{
    public Transform TpCameraPoint;
    public Transform FpCameraPoint;
    public static bool togglePov = false;
    
    [SerializeField] 
    private float rotationSpeed = 5f;
    [SerializeField]
    private float positionLerpSpeed = 10f;
    
    private float mouseX = 0f;
    private float mouseY = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY += Input.GetAxis("Mouse Y") * rotationSpeed;
        
        mouseY = Mathf.Clamp(mouseY, -45f, 45f);
        Quaternion targetRotation = Quaternion.Euler(-mouseY, TpCameraPoint.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        ChangeCamPosition();
        
    }
    
        private void ChangeCamPosition()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                togglePov = !togglePov;
            }

            // 시점에 따라 카메라 위치를 전환
            if (togglePov)
            {
                // 1인칭 시점으로 전환
                Debug.Log("Switching to First Person");
                transform.position = Vector3.Lerp(transform.position, FpCameraPoint.position, Time.deltaTime * positionLerpSpeed);
            }
            else
            {
                // 3인칭 시점으로 전환
                Debug.Log("Switching to Third Person");
                transform.position = Vector3.Lerp(transform.position, TpCameraPoint.position, Time.deltaTime * positionLerpSpeed);
            }
        }
}

