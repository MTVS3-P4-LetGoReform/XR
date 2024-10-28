using UnityEngine;

public class ModelScaling:MonoBehaviour
{
    public int subdivisionLevel = 30;
    public GameObject gameObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Bounds bounds;
    private Vector3 sourceMeshSize;
    public float scalingScale;
    void Awake()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        bounds = meshRenderer.bounds;
        sourceMeshSize = bounds.size;
    }

    public void CalcModelScale()
    {
        float maxBBoxSize = Mathf.Max(sourceMeshSize.x, sourceMeshSize.y, sourceMeshSize.z);
        scalingScale = subdivisionLevel / maxBBoxSize;
        Vector3 scaleFactor = new Vector3(scalingScale, scalingScale, scalingScale);

        Mesh mesh = Instantiate(meshFilter.mesh);
        Vector3[] vertices = mesh.vertices;
        for (int j = 0; j < vertices.Length; j++)
        {
            vertices[j] = Vector3.Scale(vertices[j], scaleFactor);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

    }
    
    
}