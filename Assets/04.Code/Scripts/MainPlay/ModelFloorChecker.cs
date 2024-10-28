using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelFloorChecker : MonoBehaviour
{
    // 한 층의 오브젝트 리스트
    public List<Vector3> voxelPos;
    public List<GameObject> voxels;
    public int maxCnt = 1;
    public int cnt = 0;
    private bool isComplete = true;
    private LayerController _layerController;
    public void Awake()
    {
        voxelPos = new List<Vector3>();
        voxels = new List<GameObject>();
        _layerController = FindObjectOfType<LayerController>();
    }

    public void Start()
    {
        
        
    }

    public void ResetVoxelPos()
    {
        voxelPos.Clear();
        voxels.Clear();
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
            voxels.Add(other.gameObject);
            cnt++;
        }

        if (cnt >= maxCnt)
        {
            Vector3 scaleFactor = new Vector3(10.0f, 10.0f, 10.0f);
            for (int i = 0; i < maxCnt; i++)
            {
                MeshFilter targetFilter = voxels[i].GetComponent<MeshFilter>();
                MeshRenderer sourceRenderer = _layerController.curFloorObjects[i].GetComponent<MeshRenderer>();
                MeshRenderer targetRenderer = voxels[i].GetComponent<MeshRenderer>();

                targetRenderer.materials = sourceRenderer.materials;  // material 전체 배열 복사
                targetFilter.sharedMesh = _layerController.curFloorObjects[i].GetComponent<MeshFilter>().sharedMesh;

                // Mesh mesh = Instantiate(targetFilter.mesh);
                // Vector3[] vertices = mesh.vertices;
                //
                // for (int j = 0; j < vertices.Length; j++)
                // {
                //     vertices[j] = Vector3.Scale(vertices[j], scaleFactor);
                // }
                //
                // mesh.vertices = vertices;
                // mesh.RecalculateBounds();
                //
                // targetFilter.mesh = mesh;
            }
            
            // foreach (GameObject vox in _layerController.curFloorObjects)
            // {
            //     vox.SetActive(true);
            // }
            // foreach (GameObject voxel in voxels)
            // {
            //     voxel.SetActive(false);
            // }
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