using System.Collections.Generic;
using MVoxelizer;
using MVoxelizer.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class C_MeshVoxelizer
{
    //settings
    public GameObject sourceGameobject = null;
    
    //voxel
    public Mesh voxelMesh = null;
    
    //single mesh
    private bool instantiateResult = true;
    
    //seperate voxels
    public Material centerMaterial = null;
    
    //voxelization data
    protected C_MVSource m_source;
    protected C_MVGrid m_grid;
    protected C_MVResult m_result;
    protected C_MVOptimization m_opt;
    protected Dictionary<C_MVInt3, C_MVVoxel> voxelDict = new Dictionary<C_MVInt3, C_MVVoxel>();

    public GameObject VoxelizeMesh(bool instantiateResult)
    {
        this.instantiateResult = instantiateResult;
        GameObject go = VoxelizeMesh();
        this.instantiateResult = true;
        return go;
    }

    public virtual GameObject VoxelizeMesh()
    {
        if (sourceGameobject == null) return null;

        GameObject result = null;
        Clear();
        if (!Initialization())
        {
            Clear();
            return null;
        }
        if (!AnaylizeMesh())
        {
            Clear();
            return null;
        }
        if (!ProcessVoxelData())
        {
            Clear();
            return null;
        }

        List<Vector3> centerVoxels = new List<Vector3>();
        if (!FillCenterSpace(centerVoxels))
        {
            Clear()
        }

    }

    void Clear()
    {
        m_source = null;
        m_grid = null;
        m_result = null;
        m_opt = null;
        voxelDict.Clear();
    }
}
