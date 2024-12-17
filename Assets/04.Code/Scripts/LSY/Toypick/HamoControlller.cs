using DG.Tweening;
using UnityEngine;

public class HamoControlller : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float duration;

    void Start()
    {
        Vector3[] path = new Vector3[] { pointA.position, pointB.position };

        // DOTween으로 이동 경로 설정
        transform.DOPath(path, duration, PathType.Linear)
            .SetLoops(-1, LoopType.Yoyo) // 무한 반복, 양방향 이동
            .SetEase(Ease.Linear) // 일정한 속도로 이동
            .OnWaypointChange(OnStepCompleted); // 포인트 도달 시 콜백 호출
    }

    void OnStepCompleted(int waypointIndex)
    {
        // 현재 위치에 따라 LookAt 대상 변경
        if (Vector3.Distance(transform.position, pointA.position) < 0.1f) // pointA 도착
        {
            Debug.Log("Arrive PointA");
            transform.DOLookAt(pointB.position, 0.5f);
        }
        else if (Vector3.Distance(transform.position, pointB.position) < 0.1f) // pointB 도착
        {
            Debug.Log("Arrive PointB");
            transform.DOLookAt(pointA.position, 0.5f);
        }
    }
}
