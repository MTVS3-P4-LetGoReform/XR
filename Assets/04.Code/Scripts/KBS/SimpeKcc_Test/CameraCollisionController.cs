using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionController : MonoBehaviour
{
    public Camera playerCamera; // 충돌을 감지하는 카메라

    public string invisibleLayerName = "Invisible"; // 변경할 레이어 이름
    public List<string> exceptionLayerNames = new List<string>(); // 예외 레이어 이름들

    private HashSet<int> exceptionLayers = new HashSet<int>(); // 예외 레이어 ID를 저장 (중복 방지)
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>(); // 오브젝트 원래 레이어 저장

    void Start()
    {
        // 예외 레이어 이름 -> 레이어 ID로 변환
        foreach (string layerName in exceptionLayerNames)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1) // 유효한 레이어만 추가
            {
                exceptionLayers.Add(layer);
            }
            else
            {
                Debug.LogWarning($"'{layerName}' 레이어는 존재하지 않습니다. 예외 레이어에서 제외됩니다.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        // 예외 레이어에 속하는 오브젝트는 무시
        if (exceptionLayers.Contains(obj.layer))
        {
            Debug.Log($"{obj.name}은(는) 예외 레이어에 속하므로 무시됩니다.");
            return;
        }

        // 이미 처리된 오브젝트인지 확인
        if (!originalLayers.ContainsKey(obj))
        {
            // 오브젝트의 원래 레이어를 저장하고 변경
            originalLayers[obj] = obj.layer;
            obj.layer = LayerMask.NameToLayer(invisibleLayerName);
            Debug.Log($"{obj.name}이(가) Invisible 레이어로 변경되었습니다.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;

        // 원래 레이어 복원
        if (originalLayers.ContainsKey(obj))
        {
            obj.layer = originalLayers[obj];
            originalLayers.Remove(obj);
            Debug.Log($"{obj.name}이(가) 원래 레이어로 복원되었습니다.");
        }
    }
}
