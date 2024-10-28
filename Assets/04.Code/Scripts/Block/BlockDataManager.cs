using Unity.Collections;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public BlockData blockData;

    public GameObject physicsBlock;

    public GameObject basicBlock;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blockData.BlockNumber = 0;
        blockData.PhysicsBlockPrefab = physicsBlock;
        blockData.BasicBlockPrefab = basicBlock;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
