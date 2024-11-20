using UnityEngine;

public class ModelgenerateController : MonoBehaviour
{
    private LayeredBoxelSystem _layeredBoxelSystem;
    private LayerController _layerController;
    private C_MeshVoxelizerWindow _cMeshVoxelizerWindow;
    private ModelScaling _modelScaling;

    private GameObject modelObject;
    private MeshFilter modelMeshFilter;
    private MeshRenderer modelMeshRenderer;
    public void Awake()
    {
        _layeredBoxelSystem = FindObjectOfType<LayeredBoxelSystem>();
        _layerController = FindObjectOfType<LayerController>();
        _cMeshVoxelizerWindow = FindObjectOfType<C_MeshVoxelizerWindow>();
        _modelScaling = FindObjectOfType<ModelScaling>();
    }

    public void GeneratePlayModel(GameObject generatedModelObject)
    {
        modelObject = generatedModelObject;
        Debug.Log($"ModelGenerateController : modelObject - {modelObject}");
        //meshObject = modelObject.transform.Find("world/geometry_0").gameObject;
        modelMeshFilter = modelObject.transform.GetComponentInChildren<MeshFilter>();
        modelMeshRenderer = modelObject.transform.GetComponentInChildren<MeshRenderer>();
        Debug.Log($"ModelGenerateController :  meshFilter - {modelMeshFilter} meshRenderer - {modelMeshRenderer}");
        Debug.Log("Scaling");
        _modelScaling.CalcModelScale(modelMeshRenderer, modelMeshFilter);
        Debug.Log("Voxelize");
        _cMeshVoxelizerWindow.Voxelize(modelMeshFilter.gameObject);
        Debug.Log("Layering");
        _layeredBoxelSystem.DoLayering(modelObject);
        _layeredBoxelSystem.DeactivateAll();
    }

    public void AdvanceLayer()
    {
        _layerController.AdvanceFloor();
    }
}
