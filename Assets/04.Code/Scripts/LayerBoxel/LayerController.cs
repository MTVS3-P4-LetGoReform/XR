using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LayerController : MonoBehaviour
{
    public BlockData blockData;
    public LayeredBoxelSystem _layeredBoxelSystem; //인스펙터 연결
    private ModelScaling ModelScaling;
    public GameObject parentGuideObject;
    private ModelFloorChecker _modelFloorChecker;

    public Material guideMat;
    private List<float> keys;
    private int curIndex = -1;
    private int progressPercent = 0;
    public LayerData curLayerdata;
    private List<GameObject> curGuideObjects;
    private OutlineRenderer outlineRenderer;

    public TMP_Text progressText;
    public Slider progressSlider;
    public GameObject pedestal;
    public List<GameObject> GarBagePileList;
    public GameObject curFloorBlocks;
    public GameObject completeFloorBlocks;
    
    public void Awake()
    {
        outlineRenderer = new OutlineRenderer();
        curGuideObjects = new List<GameObject>();
        _layeredBoxelSystem = FindObjectOfType<LayeredBoxelSystem>();
        ModelScaling = FindObjectOfType<ModelScaling>();
        _modelFloorChecker = FindObjectOfType<ModelFloorChecker>();
        curLayerdata = new LayerData();
    }

    // public void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.BackQuote))
    //     {
    //         if (Object.HasStateAuthority)
    //         {
    //             RpcAdvanceFloorMasterKey();
    //         }else {
    //             Debug.LogWarning("NetworkBehaviour가 아직 초기화되지 않았습니다.");
    //         }
    //     }
    // }

    public void Update()
    {
        if (keys != null)
        {
            progressPercent = (int)((float)(curIndex) / keys.Count *100);
            //Debug.Log($"LayerController : progressPercent - {progressPercent} curIndex - {curIndex} keys.Count - {keys.Count}");
            progressText.text = $"{progressPercent} %";
            progressSlider.value = progressPercent/100f;

            int idx = progressPercent / 10 - 1;
            if (idx >= 0 && idx < 10)
            {
                GarBagePileList[idx].SetActive(false);
            }
        }
    }

    public void AdvanceFloor()
    {
        Debug.Log("LayerController : AdvanceFloor()");
        Debug.Log($"LayController : index{curIndex}");
        
        keys = _layeredBoxelSystem.GetKeys();
        curIndex++;
        Debug.Log($"LayerController : curIndex - {curIndex}");
        if (curIndex < keys.Count)
        {
            foreach (GameObject obj in curGuideObjects)
            {
                Debug.Log("LayerController : Advance Floor Destory Guide objects");
                Destroy(obj);
            }

            curGuideObjects.Clear();
            Debug.Log($"LayerController : cuGuideObjects.Clear() {curGuideObjects} ");
            
            curLayerdata = _layeredBoxelSystem.GetLayerData(keys[curIndex]);
            //FIXME : 중복으로 계속 생기는 문제 해결
            GameObject guideObject = new GameObject("Gudieline");

            //MeshFilter meshFilter = guideObject.AddComponent<MeshFilter>();
            _modelFloorChecker.cnt = 0;
            _modelFloorChecker.ResetVoxelPos();
            int index = 0;
            foreach (GameObject voxel in curLayerdata.voxels)
            {
                if (voxel == null)
                {
                    continue;
                }
                DrawGuide(voxel, guideObject);
                _modelFloorChecker.AddVoxelPos(voxel.transform.position);
            }

            _modelFloorChecker.maxCnt = curLayerdata.maxCnt;

            guideObject.SetActive(false);
            Vector3 pPos = pedestal.transform.position;
            MoveToTarget(pedestal, new Vector3(pPos.x, pPos.y+1f, pPos.z), 1f);
            // Vector3 pPos = pedestal.transform.position;
            // pedestal.transform.position = new Vector3(pPos.x, pPos.y + 1f, pPos.z);
        }
    }
    
    // public void AdvanceFloorBtn()
    // {
    //     keys = layeredBoxelSystem.GetKeys();
    //     curIndex++;
    //     if (curIndex == 0)
    //     {
    //         layeredBoxelSystem.DeactivateAll();
    //     }
    //     else
    //     {
    //         foreach (GameObject obj in curGuideObjects)
    //         {
    //             DestroyImmediate(obj);
    //         }
    //         curGuideObjects.Clear();
    //     }
    //     
    //     curFloorObjects = layeredBoxelSystem.GetFloorObjects(keys[curIndex]);
    //     //FIXME : 중복으로 계속 생기는 문제 해결
    //     GameObject guideObject = new GameObject("Gudieline");
    //     
    //     //MeshFilter meshFilter = guideObject.AddComponent<MeshFilter>();
    //     foreach (GameObject voxel in curFloorObjects)
    //     {
    //         DrawGuide(voxel, guideObject);
    //     }
    //     guideObject.SetActive(false);
    // }

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void AdvanceFloorMasterKey()
    {
        foreach (GameObject voxel in curLayerdata.voxels)
        {
            if (voxel == null)
            {
                continue;
            }
            Debug.Log("LayerController : AdvanceFloorMasterKey()");
            var obj = RunnerManager.Instance.runner.Spawn(blockData.BasicBlockPrefab,
                voxel.transform.position, Quaternion.identity);
            var nt = obj.GetComponent<NetworkTransform>();
            nt.enabled = true;
            obj.transform.SetParent(curFloorBlocks.transform);
        }
        
    }
    public void DrawGuide(GameObject voxel, GameObject guideObject)
    {
        Mesh outlineMesh = outlineRenderer.CreateOutlineMesh(voxel.GetComponent<MeshFilter>().mesh);
        outlineRenderer.DrawOutline(outlineMesh, guideObject, guideMat);
        //float scale = ModelScaling.scaling6Scale;
        //guideObject.transform.localScale = new Vector3(scale, scale, scale);
        curGuideObjects.Add(Instantiate(guideObject, voxel.transform.position, Quaternion.identity, parentGuideObject.transform));
    }

    public void MoveToTarget(GameObject target, Vector3 targetPos, float duration)
    {
        target.transform.DOMove(targetPos, duration).SetEase(Ease.InOutSine);
    }

    public int GetRemainingFloors()
    {
        return keys.Count - curIndex;
    }
}