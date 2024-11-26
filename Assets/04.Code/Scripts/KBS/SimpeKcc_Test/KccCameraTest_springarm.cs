using Fusion;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class KccCameraTest_springarm : NetworkBehaviour
{
    public Transform TpCameraPoint;
    public Transform FpCameraPoint;
    public static bool togglePov = false;

    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private float springArmDistance = 5f;
    [SerializeField]
    private float minCameraDistance = 1f;
    [SerializeField]
    private float transitionSpeed = 5f; // 카메라 전환 속도

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float currentSpringArmDistance;
    private Vector3 currentCameraPosition;
    private bool isTransitioning = false;
    private float transitionTime = 0f;

    private bool _isLocked = false;
    private const string PlaySceneName = "Alpha_PlayScene";

    private float _zoomVelocity;
    public float characterHeight = 1.6f;
    public float pivotOffset = 0.8f;
    public LayerMask collisionLayer;

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
        currentSpringArmDistance = springArmDistance;
        currentCameraPosition = transform.position;
        collisionLayer = LayerMask.GetMask("Floor");
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
            isTransitioning = true;
            transitionTime = 0f;
        }

        Vector3 targetPosition;
        Vector3 characterCenter = transform.parent.position + Vector3.up * (characterHeight * 0.5f);

        if (togglePov)
        {
            targetPosition = FpCameraPoint.position;
        }
        else
        {
            Vector3 cameraDirection = -transform.forward;
            float targetDistance = springArmDistance;

            RaycastHit hit;
            if (Physics.Raycast(characterCenter, cameraDirection, out hit, springArmDistance, collisionLayer))
            {
                targetDistance = Mathf.Max(hit.distance, minCameraDistance);
            }

            currentSpringArmDistance = Mathf.SmoothDamp(currentSpringArmDistance, targetDistance, ref _zoomVelocity, 0.1f);
            targetPosition = characterCenter + cameraDirection * currentSpringArmDistance;
        }

        if (isTransitioning)
        {
            transitionTime += Time.deltaTime * transitionSpeed;
            float t = Mathf.SmoothStep(0f, 1f, transitionTime);
            transform.position = Vector3.Lerp(currentCameraPosition, targetPosition, t);

            if (transitionTime >= 1f)
            {
                isTransitioning = false;
                transitionTime = 0f;
            }
        }
        else
        {
            transform.position = targetPosition;
        }

        currentCameraPosition = transform.position;
    }
}
