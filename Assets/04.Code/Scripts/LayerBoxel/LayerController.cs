using System.Collections.Generic;
using UnityEngine;

public class LayerController:MonoBehaviour
{
    
    public LayeredBoxelSystem layeredBoxelSystem; //인스펙터 연결
    public ModelScaling ModelScaling;
    public GameObject parentGuideObject;

    public Material guideMat;
    private List<float> keys;
    private int curIndex = -1;
    private List<GameObject> curFloorObjects;
    private List<GameObject> curGuideObjects;
    private OutlineRenderer outlineRenderer;
    public void Start()
    {
        outlineRenderer = new OutlineRenderer();
        curGuideObjects = new List<GameObject>();
    }
    public void AdvanceFloorBtn()
    {
        keys = layeredBoxelSystem.GetKeys();
        curIndex++;
        if (curIndex == 0)
        {
            layeredBoxelSystem.DeactivateAll();
        }

        if (curGuideObjects.Count != 0)
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

    public void DrawGuide(GameObject voxel, GameObject guideObject)
    {
        Mesh outlineMesh = outlineRenderer.CreateOutlineMesh(voxel.GetComponent<MeshFilter>().mesh);
        outlineRenderer.DrawOutline(outlineMesh, guideObject, guideMat);
        float scale = ModelScaling.scalingScale;
        guideObject.transform.localScale = new Vector3(scale, scale, scale);
        curGuideObjects.Add(Instantiate(guideObject, voxel.transform.position, Quaternion.identity, parentGuideObject.transform));
    }
}