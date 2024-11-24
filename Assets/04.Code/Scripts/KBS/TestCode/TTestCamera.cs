using UnityEngine;

public class TTestCamera : MonoBehaviour
{
    public float Yaxis;
    public float Xaxis;
    public Transform target;
    public Camera mainCamera; // 메인 카메라 참조

    private float rotSensitive = 3f;
    private float dis = 2f;
    private float RotationMin = -10f;
    private float RotationMax = 80f;
    private float smoothTime = 0.12f;
    private Vector3 targetRotation;
    private Vector3 currentVel;

    // 줌 관련 변수
    private float currentZoom;
    private float targetZoom;
    private float zoomSmoothTime = 0.15f;
    private float zoomSmoothVelocity;
    private float zoomSpeed = 2f;
    private float minZoom = 0.5f;
    private float maxZoom = 5f;

    // 시점 전환 관련 변수
    private bool isFirstPerson = false;
    private float thirdPersonDis;
    private Vector3 firstPersonOffset = new Vector3(0, 0.5f, 0);

    // 줌 보간 관련 변수
    private float currentZoomVelocity;

    void Start()
    {
        currentZoom = dis;
        targetZoom = dis;
        thirdPersonDis = dis;

        // 레이어 마스크 설정
        if (mainCamera != null)
        {
            // 플레이어 레이어를 제외한 모든 레이어 표시
            mainCamera.cullingMask = ~(1 << LayerMask.NameToLayer("Player"));
        }
    }

    void Update()
    {
        // 시점 전환
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isFirstPerson = !isFirstPerson;
            targetZoom = isFirstPerson ? -0.2f : thirdPersonDis;
        }

        // 3인칭 시점일 때만 줌 조절 가능
        if (!isFirstPerson)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            targetZoom -= scrollInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // 부드러운 줌 적용
        currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref currentZoomVelocity, zoomSmoothTime);
        dis = currentZoom;

        // 카메라 회전에 따른 캐릭터 회전 (180도 반대 방향)
        if (!isFirstPerson)
        {
            float targetAngle = transform.eulerAngles.y + 180f; // 180도 더해서 반대 방향 보기
            target.rotation = Quaternion.Euler(0, targetAngle, 0);
        }
    }

    void LateUpdate()
    {
        // 카메라 회전
        Yaxis += Input.GetAxis("Mouse X") * rotSensitive;
        Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive;
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        transform.eulerAngles = targetRotation;

        // 카메라 위치 설정
        if (isFirstPerson)
        {
            transform.position = target.position + firstPersonOffset;
        }
        else
        {
            transform.position = target.position - transform.forward * dis;
        }
    }

}
