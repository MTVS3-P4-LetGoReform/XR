using UnityEngine;

public class GameMasterContorller : MonoBehaviour
{
    private GameObject voxelizedObjects;
    public GameObject parentObject;
    public GameObject orgVoxels;
    public GameObject guideObjects;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SetComplete();
        }
    }

    private void SetComplete()
    {
        voxelizedObjects = parentObject.transform.GetChild(0).gameObject;
        guideObjects.SetActive(false);
        orgVoxels.SetActive(false);
        foreach (Transform child in voxelizedObjects.transform)
        {
            child.gameObject.SetActive(true);
        }
        
    }
}
