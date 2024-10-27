using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelFloorChecker : MonoBehaviour
{
    // 한 층의 오브젝트 리스트
    public List<Vector3> voxelPos;
    private int maxCnt = 0;
    private int cnt = 0;
    private bool isComplete = true;

    public void Awake()
    {
    }

    public void Start()
    {
        voxelPos = new List<Vector3>();
        
    }

    public void RestVoxelPos()
    {
        voxelPos.Clear();
        maxCnt = 0;
    }
    public void AddVoxelPos(Vector3 pos)
    {
        voxelPos.Add(pos);
        maxCnt++;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("");
        if (other.CompareTag("Block"))
        {
            cnt++;
        }

        if (cnt < maxCnt)
        {
            
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