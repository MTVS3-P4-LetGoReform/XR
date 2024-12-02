using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelPlacementChecker : MonoBehaviour
{
    // 한 층의 오브젝트 리스트
    //public List<GameObject> voxelList;
    //private int maxCnt = 0;
    //private int cnt = 0;
    //private bool isComplete = true;
    private LayerController _layerController;

    public void Awake()
    {
        _layerController = FindObjectOfType<LayerController>();
    }

    public void Start()
    {
    //     voxelList = _layerController.curFloorObjects;
    //     maxCnt = voxelList.Count;
         //StartCheckFloorComplete();
    }

    // public void Initialize()
    // {
    //     //voxelList = _layerController.curFloorObjects;
    //     //maxCnt = voxelList.Count;
    //     //StartCheckFloorComplete();
    // }
    // public void StartCheckFloorComplete()
    // {
    //     StartCoroutine(CheckFloorComplete());
    // }


    public bool CheckValidation(Vector3 pos)
    {
        foreach (GameObject voxel in _layerController.curLayerdata.voxels)
        {
            if (voxel == null)
            {
                continue;
            }
            if (voxel.transform.position == pos)
            {
                // cnt++;
                // if (cnt == maxCnt)
                // {
                //     isComplete = true;
                // }
                return true;
            }
        }
        return false;
    }


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
}