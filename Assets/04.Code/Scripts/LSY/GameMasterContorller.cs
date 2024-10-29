using UnityEngine;

public class GameMasterContorller : MonoBehaviour
{
    private GameObject voxelizedObjects;
    public GameObject parentObject;
    public GameObject orgVoxels;

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            GameStateManager.Instance.isComplete = true;
            orgVoxels.SetActive(false);
            SetComplete();
        }
    }

    private void SetComplete()
    {
        voxelizedObjects = parentObject.transform.GetChild(0).gameObject;
        
        foreach (Transform child in voxelizedObjects.transform)
        {
            child.gameObject.SetActive(true);
        }
        
    }
}
