using Fusion;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TTTCamera : MonoBehaviour
{
    public Transform TpCameraPoint;
    public Transform FpCameraPoint;
    public static bool togglePov = false;
    
    [SerializeField] 
    private float rotationSpeed = 5f;
    [SerializeField]
    private float positionLerpSpeed = 10f;

    private Quaternion _cameraLockedRotation;
    private Vector3 currentVel;
    private float smoothTime = 0.12f;
    
    private float mouseX = 0f;
    private float mouseY = 0f;
    
    private bool _isLocked = false;
    
    private void LateUpdate()
    {
        if (_isLocked)
            return;
        
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        mouseY = Mathf.Clamp(mouseY, -45f, 45f);
        
        Quaternion targetRotation = Quaternion.Euler(mouseY, TpCameraPoint.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        ChangeCamPosition();
        
    }
    
    
    private void ChangeCamPosition()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            togglePov = !togglePov;
        }

        Vector3 targetPosition;


        // 시점에 따라 카메라 위치를 전환
        if (togglePov)
        {
            // 1인칭 시점으로 전환
            //Debug.Log("Switching to First Person");
            targetPosition = FpCameraPoint.position;
            
           /* transform.position = Vector3.Lerp(transform.position, FpCameraPoint.position,
                Time.deltaTime * positionLerpSpeed); */
        }
        else
        {
            // 3인칭 시점으로 전환
            //Debug.Log("Switching to Third Person");
            /*transform.position = Vector3.Lerp(transform.position, TpCameraPoint.position,
                Time.deltaTime * positionLerpSpeed); */
            
            targetPosition = TpCameraPoint.position;

            
            float adjustedHeight = Mathf.Lerp(0f, 1.5f, (-mouseY + 45f) / 90f); // 회전 각도에 따라 카메라 높이 조정
            targetPosition.y -= adjustedHeight;
        }
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionLerpSpeed);
    }
}
