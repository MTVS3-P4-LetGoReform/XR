using System.Collections.Generic;
using MVoxelizer.Util;
using Unity.VisualScripting;
using UnityEngine;

public class C_MeshVoxelizerWindow : MonoBehaviour
{
    public class C_TargetInfo
    {
        public GameObject go;
        public Mesh mesh = null;
        public Vector3 meshBBoxSize = new Vector3();
        public MVInt3 voxelBBoxSize = new MVInt3();
        public MeshRenderer meshRenderer = null;
        public SkinnedMeshRenderer skinnedMeshRenderer = null;
        public float maxBBoxSize = 0.0f;

        public C_TargetInfo(GameObject go)
        {
            this.go = go;
            meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                mesh = go.GetComponent<MeshFilter>().sharedMesh;
            }
            else
            {
                skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    mesh = skinnedMeshRenderer.sharedMesh;
                }
            }
        }

        public string GetMeshBBoxString()
        {
            string x = GetFloatString(meshBBoxSize.x);
            string y = GetFloatString(meshBBoxSize.y);
            string z = GetFloatString(meshBBoxSize.z);
            return "X:" + x + " Y:" + y + " Z:" + z;
        }

        public string GetVoxelBBoxString()
        {
            return "X:" + voxelBBoxSize.x + " Y:" + voxelBBoxSize.y + " Z:" + voxelBBoxSize.z;
        }

        string GetFloatString(float f)
        {
            return Mathf.Abs(f) < 1.0f ? f.ToString("f2") :
                Mathf.Abs(f) < 10.0f ? f.ToString("f1") :
                f.ToString("f0");
        }
    }
    public C_MeshVoxelizerEditor c_meshVoxelizer;
    //public bool doVoxelization = false; // dovoxelization 실행하는 시점 판별용

    public Dictionary<GameObject, C_TargetInfo> targetDict = new Dictionary<GameObject, C_TargetInfo>();
    public void ClickBoxelizeBtn()
    {
        Debug.Log("ClickBoxelizationBtn");
        InitVoxelizer();
        DoVozelization();
        // 기존 선언 위치bool doVoxelization = GUILayoout.Button(vbText, vbOptions); 
        
    }
    
    public void Voxelize()
    {
        InitVoxelizer();
        DoVozelization();
    }
    // voxelizer 초기화
    void InitVoxelizer()
    {
        
        if (c_meshVoxelizer != null) return;

        //c_meshVoxelizer = new C_MeshVoxelizerEditor();
        c_meshVoxelizer.centerMaterial = C_MVHelper.GetDefaultVoxelMaterial();
        c_meshVoxelizer.voxelMesh = C_MVHelper.GetDefaultVoxelMesh();
    }
    // voxelize함수 호출 및 타겟 모델 설정
    public void DoVoxelization()
    {
        foreach (C_TargetInfo target in targetDict.Values)
        {
            c_meshVoxelizer.sourceGameObject = target.go;
            c_meshVoxelizer.VoxelizeMesh();
        }
    }
    
    void DoVozelization()
    {
        Debug.Log("DoVozelization");
        //foreach(C_TargetInfo target in targetDict.Values)
        //{
        //    c_meshVoxelizer.sourceGameObject = target.go;
            c_meshVoxelizer.VoxelizeMesh();
        //}
    }



}
