using Fusion;
using UnityEngine;

public class GameMasterContorller : NetworkBehaviour
{
    //private GameObject voxelizedObjects;
    //public GameObject parentObject;
    private LayerController _layerController;

    void Start()
    {
        _layerController = FindObjectOfType<LayerController>();
    }
    void Update()
    {
        if (!HasStateAuthority)
            return;
        
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            // GameStateManager.Instance.isComplete = true;
            // orgVoxels.SetActive(false);
            // SetComplete();
            AdvanceFloorMasterRpc();
        }
    }

    // private void SetComplete()
    // {
    //     voxelizedObjects = parentObject.transform.GetChild(0).gameObject;
    //     
    //     foreach (Transform child in voxelizedObjects.transform)
    //     {
    //         child.gameObject.SetActive(true);
    //     }
    // }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void AdvanceFloorMasterRpc()
    {
        _layerController.AdvanceFloorMasterKey();
    }
}
