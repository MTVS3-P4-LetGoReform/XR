using System.Collections.Generic;
using MVoxelizer.Util;
using UnityEngine;

public class C_MeshVoxelizerWindow : MonoBehaviour
{
    private C_MeshVoxelizerEditor c_meshVoxelizer;
    //public bool doVoxelization = false; // dovoxelization 실행하는 시점 판별용
    
    Dictionary<GameObject, C_TargetInfo> targetDict = new Dictionary<GameObject, C_TargetInfo>()
    public void Start()
    {
        InitVoxelizer();

        // 기존 선언 위치bool doVoxelization = GUILayoout.Button(vbText, vbOptions); 
    }
    // voxelizer 초기화
    void InitVoxelizer()
    {
        if (c_meshVoxelizer != null) return;

        c_meshVoxelizer = new C_MeshVoxelizerEditor();
        c_meshVoxelizer.centerMaterial = C_MVHelper.GetDefaultVoxelMaterial();
        c_meshVoxelizer.voxelMesh = C_MVHelper.GetDefaultVoxelMesh();
    }
    // voxelize함수 호출 및 타겟 모델 설정
    public void DoVoxelization()
    {
        foreach (C_TargetInfo target in targetDict.Values)
        {
            c_meshVoxelizer.sourceGameobject = target.go;
            c_meshVoxelizer.VoxelizeMesh();
        }
    }



}
