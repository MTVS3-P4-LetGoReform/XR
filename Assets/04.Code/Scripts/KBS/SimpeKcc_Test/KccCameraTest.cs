using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class KccCameraTest : NetworkBehaviour
{
    public Transform TpCameraPoint;
    public Transform FpCameraPoint;
    public static bool togglePov = false;
    public static bool toggleFaceCam = false;
    public MonoBehaviour collisionControllerScript;
    
    [SerializeField] 
    private float rotationSpeed = 5f;
    [SerializeField]
    private float positionLerpSpeed = 10f;

    private Quaternion _cameraLockedRotation;
    
    private float mouseX = 0f;
    private float mouseY = 0f;
    
    private bool _isLocked = false;
    private const string PlaySceneName = "Alpha_PlayScene";
    
    void Start()
    {
        if (!HasStateAuthority)
        {
            Destroy(gameObject);
        }
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInput.OnChat += CameraLock;
        PlayerInput.OnMessenger += CameraLock;
        PlayerInput.OnMouse += CameraLock;
        if (SceneManager.GetActiveScene().name == PlaySceneName)
        {
            GameStateManager.Instance.Complete += CameraLock;
        }
    }

    private void CameraLock(bool isActive)
    {
        if (!isActive)
        {
            rotationSpeed = 5f;
            _isLocked = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            rotationSpeed = 0f;
            _isLocked = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void LateUpdate()
    {
        if (_isLocked)
            return;
        
        mouseY += Input.GetAxis("Mouse Y") * rotationSpeed;

        mouseY = Mathf.Clamp(mouseY, -45f, 75f);
        
        Quaternion targetRotation = Quaternion.Euler(-mouseY, TpCameraPoint.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        ChangeCamPosition();
    }
    
    
    private void ChangeCamPosition()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            togglePov = !togglePov;
            collisionControllerScript.enabled = !collisionControllerScript.enabled;
        }

        Vector3 targetPosition;

        // 시점에 따라 카메라 위치를 전환
        if (togglePov)
        {
            // 1인칭 시점으로 전환
            //Debug.Log("Switching to First Person");
           /* transform.position = Vector3.Lerp(transform.position, FpCameraPoint.position,
                Time.deltaTime * positionLerpSpeed); */

           targetPosition = FpCameraPoint.position;
        }
        else
        {
            // 3인칭 시점으로 전환
            //Debug.Log("Switching to Third Person");
            /* transform.position = Vector3.Lerp(transform.position, TpCameraPoint.position,
                Time.deltaTime * positionLerpSpeed); */
            targetPosition = TpCameraPoint.position;
            
            float adjustedHeight = Mathf.Lerp(3f, 0f, (-mouseY + 45f) / 75f); // 회전 각도에 따라 카메라 높이 조정
            targetPosition.y -= adjustedHeight;
            

        }
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionLerpSpeed);
    }
    
}

