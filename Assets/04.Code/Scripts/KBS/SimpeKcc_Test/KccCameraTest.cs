using Fusion;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;

public class KccCameraTest : NetworkBehaviour
{
    public Transform TpCameraPoint;
    public Transform FpCameraPoint;
    public static bool togglePov = false;
    
    [SerializeField] 
    private float rotationSpeed = 5f;
    [SerializeField]
    private float positionLerpSpeed = 10f;

    private Quaternion cameraLockedRotation;
    
    private float mouseX = 0f;
    private float mouseY = 0f;

    private bool _onChat = false;
    private bool isLocked = false;
    private bool isTapKeyPressed = false;
    
    void Start()
    {
        if (!HasStateAuthority)
        {
            Destroy(gameObject);
        }
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInput.OnChat += CameraLock;
        
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            var readyCheck = FindAnyObjectByType<ReadyCheck>();
            readyCheck.gameStartButton.gameObject.SetActive(true);

            GameStateManager.Instance.Complete += CameraLock;
        }
    }

    private void CameraLock(bool onChat)
    {
        if (!onChat)
        {
            rotationSpeed = 5f;
            _onChat = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            rotationSpeed = 0f;
            _onChat = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Update()
    {
        if (_onChat)
            return;
        
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY += Input.GetAxis("Mouse Y") * rotationSpeed;

        mouseY = Mathf.Clamp(mouseY, -45f, 45f);
        Quaternion targetRotation = Quaternion.Euler(-mouseY, TpCameraPoint.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        ChangeCamPosition();
        MousePointController();
        
    }

    private void MousePointController()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            isTapKeyPressed = true;
            rotationSpeed = 0f;
            
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isTapKeyPressed = false;
            rotationSpeed = 5f;
            
        }

        if (isTapKeyPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
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
            //Debug.Log("Switching to First Person");
            transform.position = Vector3.Lerp(transform.position, FpCameraPoint.position,
                Time.deltaTime * positionLerpSpeed);
        }
        else
        {
            // 3인칭 시점으로 전환
            //Debug.Log("Switching to Third Person");
            transform.position = Vector3.Lerp(transform.position, TpCameraPoint.position,
                Time.deltaTime * positionLerpSpeed);
        }
    }
}

