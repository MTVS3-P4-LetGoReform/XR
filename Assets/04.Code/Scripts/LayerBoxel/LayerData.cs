using UnityEngine;

public class LayerData
{
    public int maxCnt;
    public GameObject[,] voxels;

    public LayerData()
    {
        voxels = new GameObject[MainPlayConstants.maxLayeredVoxelNum, MainPlayConstants.maxLayeredVoxelNum];
    }
}