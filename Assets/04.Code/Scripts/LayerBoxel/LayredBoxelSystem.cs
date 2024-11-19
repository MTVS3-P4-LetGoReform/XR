
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class LayeredBoxelSystem : MonoBehaviour
{
    public GameObject parentObject;
    private GameObject boxelizedObject;
    private SortedDictionary<float, List<GameObject>> voxelList;
    //Dictionary<float, List<GameObject>>
    // scaling 관련

    public void Start()
    {
        parentObject = GameObject.FindWithTag("GLBModel");
    }
    public void DoLayering(GameObject modelObject)
    {
        parentObject = modelObject;
        boxelizedObject = parentObject.transform.GetChild(0).gameObject;
        GameStateManager.Instance.maxCnt = boxelizedObject.transform.childCount;
        Debug.Log("LayeredBoxelSystem : 총 복셀 수 - "+ GameStateManager.Instance.allCnt);
        voxelList = new SortedDictionary<float, List<GameObject>>();
        Vector3 pos;
        foreach (Transform child in boxelizedObject.transform)
        {
            pos = new Vector3(Mathf.Floor(child.position.x), Mathf.Floor(child.position.y),
                Mathf.Floor(child.position.z));
            pos += Vector3.one * 0.5f;
            child.position = pos;
            AddVoxel(child.position.y, child.gameObject);
        }
    }
    // public void LayeringBtn()
    // {
    //     boxelizedObject = parentObject.transform.GetChild(0).gameObject;
    //     voxelList = new Dictionary<float, List<GameObject>>();
    //     //Vector3 pos;
    //     foreach (Transform child in boxelizedObject.transform)
    //     {
    //         // pos = new Vector3(Mathf.Floor(child.position.x), Mathf.Floor(child.position.y),
    //         //     Mathf.Floor(child.position.z));
    //         // pos += Vector3.one * 0.5f;
    //         // child.position = pos;
    //         AddVoxel(child.position.y, child.gameObject);
    //     }
    //
    //     PrintVoxels();
    // }
    public  SortedDictionary<float, List<GameObject>> Layering()
    {
        boxelizedObject = parentObject.transform.GetChild(0).gameObject;
        voxelList = new SortedDictionary<float, List<GameObject>>();
        foreach (Transform child in boxelizedObject.transform)
        {
            AddVoxel(child.position.y, child.gameObject);
        }

        PrintVoxels();
        return voxelList;
    }

    public void AddVoxel(float key, GameObject voxel)
    {
        if (voxelList.ContainsKey(key)== false)
        {
            Debug.Log("key : "+key);
            voxelList[key] = new List<GameObject>();
        }

        voxelList[key].Add(voxel);
    }

    public void PrintVoxels()
    {
        foreach (var pair in voxelList)
        {
            Debug.Log($"Key : {pair.Key}, counts : {pair.Value.Count}");
        }
    }

    public void DeactivateAll()
    {
        foreach (Transform child in boxelizedObject.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public List<float> GetKeys()
    {
        return voxelList.Keys.ToList();
    }

    public List<GameObject> GetFloorObjects(float key)
    {
        return voxelList[key];
    }

}