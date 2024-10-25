using UnityEngine;

public class ModelScaling:MonoBehaviour
{
    public int subdivisionLevel = 30;
    public GameObject gameObject;
    private MeshRenderer meshRenderer;
    private Bounds bounds;
    private Vector3 sourceMeshSize;
    public float scalingScale;
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        bounds = meshRenderer.bounds;
        sourceMeshSize = bounds.size;
    }

    public void CalcModelScale()
    {
        float maxBBoxSize = Mathf.Max(sourceMeshSize.x, sourceMeshSize.y, sourceMeshSize.z);
        scalingScale = subdivisionLevel / maxBBoxSize;
        gameObject.transform.localScale = new Vector3(scalingScale, scalingScale, scalingScale);
    }
    
    
}