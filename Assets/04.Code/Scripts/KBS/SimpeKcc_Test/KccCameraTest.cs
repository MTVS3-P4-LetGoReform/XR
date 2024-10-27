
using UnityEngine;

public class KccCameraTest : MonoBehaviour
{
    public Transform TpcameraPoint;
    public Transform FpCameraPoint;
    
    [SerializeField] 
    private float rotationSpeed = 5f;

    [SerializeField]
    private float positionLerpSpeed = 10f;

    private bool togglePov = false;
    
    private float mouseX = 0f;
    private float mouseY = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY += Input.GetAxis("Mouse Y") * rotationSpeed;

        mouseY = Mathf.Clamp(mouseY, -45f, 45f);
        transform.rotation = Quaternion.Euler(-mouseY, TpcameraPoint.eulerAngles.y, 0f);
        
       /* if (togglePov)
        {
            transform.rotation = Quaternion.Euler(-mouseY, FpCameraPoint.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(-mouseY, TpcameraPoint.eulerAngles.y, 0f);
        } */
        
        ChangeCamPosition();
        
    }
    
        private void ChangeCamPosition()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                togglePov = !togglePov;
                Debug.Log("Toggle POV: " + togglePov); // 현재 토글 상태 확인
            }

            // 시점에 따라 카메라 위치를 즉시 전환
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
                transform.position = Vector3.Lerp(transform.position, TpcameraPoint.position, Time.deltaTime * positionLerpSpeed);
            }
        }
}

