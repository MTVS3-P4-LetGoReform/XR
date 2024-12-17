using System.Collections;
using System.Windows.Forms;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMasterContorller : NetworkBehaviour
{
    //private GameObject voxelizedObjects;
    //public GameObject parentObject;
    private LayerController _layerController;
    public PlayerStatus _playerStatus;

    void Start()
    {
        _layerController = FindObjectOfType<LayerController>();
        _playerStatus = FindObjectOfType<PlayerStatus>();
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
            return;

        if (!HasStateAuthority)
            return;
        
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            // GameStateManager.Instance.isComplete = true;
            // orgVoxels.SetActive(false);
            // SetComplete();
            AdvanceFloorMasterRpc();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AdvanceRemainingFloorsMasterRpc();
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

    //[Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    private void AdvanceFloorMasterRpc()
    {
        _layerController.AdvanceFloorMasterKey();
    }

    private void AdvanceRemainingFloorsMasterRpc()
    {
        StartCoroutine(AdvanceRemainingFloors());
    }
    IEnumerator AdvanceRemainingFloors()
    {
        int remaining = _layerController.keys.Count - _layerController.curIndex;
        for (int i = 0; i < remaining; i++)
        {
            _layerController.AdvanceFloorMasterKey();
            yield return new WaitForSeconds(0.1f);
        }
    }
}//
