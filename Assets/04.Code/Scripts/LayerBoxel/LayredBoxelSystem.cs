using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.Switch;
using UnityEngine.UI;

public class LayeredBoxelSystem : MonoBehaviour
{
    public GameObject parentObject;
    private GameObject boxelizedObject;
    private Dictionary<float, List<GameObject>> voxelList;
    //Dictionary<float, List<GameObject>>
    
    // scaling 관련
    

    
    
    public void LayeringBtn()
    {
        boxelizedObject = parentObject.transform.GetChild(0).gameObject;
        voxelList = new Dictionary<float, List<GameObject>>();
        foreach (Transform child in boxelizedObject.transform)
        {
            AddVoxel(child.position.y, child.gameObject);
        }

        PrintVoxels();
    }
    public  Dictionary<float, List<GameObject>> Layering()
    {
        boxelizedObject = parentObject.transform.GetChild(0).gameObject;
        voxelList = new Dictionary<float, List<GameObject>>();
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