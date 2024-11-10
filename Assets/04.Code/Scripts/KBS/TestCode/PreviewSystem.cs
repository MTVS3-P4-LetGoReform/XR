using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = System.Drawing.Color;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject previewObject;

    [SerializeField] 
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;
    
    void Start()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }

            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        MovePreview(position);
        MoveCursor(position);
        ApplyFeedback(validity);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            //
        }
    }

    private void MovePreview(Vector3 position)
    {
        
    }

    private void MoveCursor(Vector3 position)
    {
        
    }

    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.White : Color.Red;
        //c.A = 0.5f;
       // previewMaterialInstance.color = c;

    }
    
    void Update()
    {
        
    }
}
