using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelFloorChecker : MonoBehaviour
{
    // 한 층의 오브젝트 리스트
    public List<Vector3> voxelPos;
    public int maxCnt = 1;
    public int cnt = 0;
    private bool isComplete = true;
    private LayerController _layerController;
    public void Awake()
    {
        voxelPos = new List<Vector3>();
        _layerController = FindObjectOfType<LayerController>();
    }

    public void Start()
    {
        
        
    }

    public void ResetVoxelPos()
    {
        voxelPos.Clear();
        maxCnt = 0;
    }
    public void AddVoxelPos(Vector3 pos)
    {
        voxelPos.Add(pos);
        //maxCnt++;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        if (other.CompareTag("Block"))
        {
            Debug.Log("ModelFloorChecker : Block Place Trigger");
            cnt++;
        }

        if (cnt >= maxCnt)
        {
            Debug.Log("ModelFloorChecker : AdvanceFloor");
            _layerController.AdvanceFloor();
            //ResetVoxelPos();
            //cnt = 0;

            // foreach (GameObject obj in _layerController.curFloorObjects)
            // {
            //     AddVoxelPos(obj.transform.position);
            // }
            // maxCnt = voxelPos.Count;
        }
    }
}
    
    // public void Initialize()
    // {
    //     voxelList = _layerController.curFloorObjects;
    //     maxCnt = voxelList.Count;
    //     StartCheckFloorComplete();
    // }
    // public void StartCheckFloorComplete()
    // {
    //     StartCoroutine(CheckFloorComplete());
    // }


    // public bool CheckValidation(Vector3 pos)
    // {
    //     foreach (GameObject voxel in voxelList)
    //     {
    //         if (voxel.transform.position == pos)
    //         {
    //             cnt++;
    //             if (cnt == maxCnt)
    //             {
    //                 isComplete = true;
    //             }
    //             return true;
    //         }
    //     }
    //     return false;
    // }


    // private IEnumerator CheckFloorComplete()
    // {
    //     while (true)
    //     {
    //         yield return new WaitUntil(() => isComplete == true);
    //         isComplete = false;
    //         _layerController.AdvanceFloor();
    //         // FIXME : 텍스처 갈아끼우기 추가
    //         voxelList = _layerController.curFloorObjects;
    //         maxCnt = voxelList.Count;
    //         cnt = 0;
    //     }
    // }