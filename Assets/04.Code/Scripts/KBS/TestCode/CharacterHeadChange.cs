using UnityEngine;

public class CharacterHeadChange : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] 
    private CustomDatabase customDatabase;
    private int selectedCustomIndex = -1;


    public void ChangeMesh(int ID)
    {
        selectedCustomIndex =
            customDatabase.customData.FindIndex(data => data.ID == ID);

       //skinnedMeshRenderer.sharedMesh = customDatabase.customData[selectedCustomIndex].meshes;

    }
    
}
