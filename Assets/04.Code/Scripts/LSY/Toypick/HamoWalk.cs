using DG.Tweening;
using UnityEngine;

public class HamoWalk : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float duration = 2f;

    void Start()
    {
        Vector3[] path = new Vector3[] { pointA.position, pointB.position };

        // DOTween으로 이동 경로 설정
        transform.DOPath(path, duration, PathType.Linear)
            .SetLoops(-1, LoopType.Yoyo) // 무한 반복, 양방향 이동
            .SetEase(Ease.Linear) // 일정한 속도로 이동
            .OnWaypointChange(OnWaypointReached); // 포인트 도달 시 콜백 호출
    }

    void OnWaypointReached(int waypointIndex)
    {
        // 도달한 포인트에 따라 LookAt 대상을 변경
        if (waypointIndex == 0) // pointA에 도달
        {
            transform.DOLookAt(pointB.position, 0.5f); // A 대상 바라보기
        }
        else if (waypointIndex == 1) // pointB에 도달
        {
            transform.DOLookAt(pointA.position, 0.5f); // B 대상 바라보기
        }
    }
    // public Transform[] pathPoints; // 이동 경로의 포인트
    // public float duration = 10f; // 이동에 걸리는 시간
    //
    // void Start()
    // {
    //     // 경로 포인트 배열로 경로 생성
    //     Vector3[] path = new Vector3[pathPoints.Length];
    //     for (int i = 0; i < pathPoints.Length; i++)
    //     {
    //         path[i] = pathPoints[i].position;
    //     }
    //
    //     // DOPath로 이동 및 회전 설정
    //     transform.DOPath(path, duration, PathType.CatmullRom) // 부드러운 곡선 경로
    //         .SetLookAt(0.01f) // 이동 방향으로 회전 (LookAt 설정)
    //         .SetEase(Ease.Linear); // 일정한 속도로 이동
    // }
    // public float initPositionZ;
    // // Light Reft Moving
    // public bool turnSwitch;
    // public float moveSpeed;
    // public float turningPoint;
    // public float distance;
    // //RT_Floor
    // public float rotateSpeed;
    //
    // public void leftRight()
    // {
    //     float currentPositionZ = transform.position.z;
    //
    //     if (currentPositionZ >= initPositionZ - distance)
    //     {
    //         turnSwitch = false;
    //     }
    //     
    //     else if (currentPositionZ <= turningPoint)
    //     {
    //         turnSwitch = true;
    //     }
    //
    //     if (turnSwitch)
    //     {
    //         transform.position = transform.position + new Vector3(0, 0, -1) * moveSpeed * Time.deltaTime;
    //     }
    //
    //     else
    //     {
    //         transform.position = transform.position - new Vector3(0, 0, 1) * moveSpeed * Time.deltaTime;
    //     }
    // }
}
