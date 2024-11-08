using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Proto_LayerController:MonoBehaviour
{
    
    private LayeredBoxelSystem layeredBoxelSystem; //인스펙터 연결
    private ModelScaling ModelScaling;
    public GameObject parentGuideObject;
    private ModelFloorChecker _modelFloorChecker;

    public Material guideMat;
    private List<float> keys;
    private int curIndex = -1;
    public List<GameObject> curFloorObjects;
    private List<GameObject> curGuideObjects;
    private OutlineRenderer outlineRenderer;

    public GameObject pedestal;
    
    public void Awake()
    {
        outlineRenderer = new OutlineRenderer();
        curGuideObjects = new List<GameObject>();
        layeredBoxelSystem = FindObjectOfType<LayeredBoxelSystem>();
        ModelScaling = FindObjectOfType<ModelScaling>();
        _modelFloorChecker = FindObjectOfType<ModelFloorChecker>();
    }

    public void AdvanceFloor()
    {
        Debug.Log("LayerController : AdvanceFloor()");
        keys = layeredBoxelSystem.GetKeys();
        curIndex++;
        if (curIndex < keys.Count)
        {
            if (curIndex == 0)
            {
                layeredBoxelSystem.DeactivateAll();
            }

            else
            {
                foreach (GameObject obj in curGuideObjects)
                {
                    Destroy(obj);
                }

                curGuideObjects.Clear();
            }

            curFloorObjects = layeredBoxelSystem.GetFloorObjects(keys[curIndex]);
            //FIXME : 중복으로 계속 생기는 문제 해결
            GameObject guideObject = new GameObject("Gudieline");

            MeshFilter meshFilter = guideObject.AddComponent<MeshFilter>();
            _modelFloorChecker.cnt = 0;
            _modelFloorChecker.ResetVoxelPos();
            int index = 0;
            foreach (GameObject voxel in curFloorObjects)
            {
                
                DrawGuide(voxel, guideObject);
                _modelFloorChecker.AddVoxelPos(voxel.transform.position);
            }

            _modelFloorChecker.maxCnt = curFloorObjects.Count;

            guideObject.SetActive(false);
            Vector3 pPos = pedestal.transform.position;
            MoveToTarget(pedestal, new Vector3(pPos.x, pPos.y+1f, pPos.z), 1f);
            // Vector3 pPos = pedestal.transform.position;
            // pedestal.transform.position = new Vector3(pPos.x, pPos.y + 1f, pPos.z);
        }
    }
    
    public void AdvanceFloorBtn()
    {
        keys = layeredBoxelSystem.GetKeys();
        curIndex++;
        if (curIndex == 0)
        {
            layeredBoxelSystem.DeactivateAll();
        }
        else
        {
            foreach (GameObject obj in curGuideObjects)
            {
                DestroyImmediate(obj);
            }
            curGuideObjects.Clear();
        }
        
        curFloorObjects = layeredBoxelSystem.GetFloorObjects(keys[curIndex]);
        //FIXME : 중복으로 계속 생기는 문제 해결
        GameObject guideObject = new GameObject("Gudieline");
        
        MeshFilter meshFilter = guideObject.AddComponent<MeshFilter>();
        foreach (GameObject voxel in curFloorObjects)
        {
            DrawGuide(voxel, guideObject);
        }
        guideObject.SetActive(false);
    }

    public void AdvanceFloorMasterKey()
    {
        keys = layeredBoxelSystem.GetKeys();
        curIndex++;
        if (curIndex == 0)
        {
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
}