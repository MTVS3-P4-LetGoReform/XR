using Fusion;
using Unity.Collections;
using UnityEngine;

public class BlockDataManager : NetworkBehaviour
{
    public BlockData blockData;

    public GameObject physicsBlock;

    public GameObject basicBlock;
    
    void Start()
    {
        blockData.BlockNumber = 0;
        blockData.PhysicsBlockPrefab = physicsBlock;
        blockData.BasicBlockPrefab = basicBlock;
    }
}
