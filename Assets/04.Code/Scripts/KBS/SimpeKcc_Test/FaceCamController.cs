using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaceCamController : NetworkBehaviour
{
    public Transform FaceCamPoint;
    public Camera FaceCamera;
    [SerializeField] 
    private float rotationSpeed = 5f;
    [SerializeField]
    private float positionLerpSpeed = 10f;

    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minFOV = 20f;
    [SerializeField] private float maxFOV = 80f;
    

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
        
        Quaternion targetRotation = Quaternion.Euler(-mouseY, FaceCamPoint.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        FaceCamera.fieldOfView -= scrollInput * zoomSpeed;
        FaceCamera.fieldOfView = Mathf.Clamp(FaceCamera.fieldOfView, minFOV, maxFOV);

    }
}
