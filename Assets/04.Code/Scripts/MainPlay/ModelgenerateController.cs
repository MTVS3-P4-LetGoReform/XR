using UnityEngine;

public class ModelgenerateController : MonoBehaviour
{
    private LayeredBoxelSystem _layeredBoxelSystem;
    private LayerController _layerController;
    private C_MeshVoxelizerWindow _cMeshVoxelizerWindow;
    private ModelScaling _modelScaling;

    public void Awake()
    {
        _layeredBoxelSystem = FindObjectOfType<LayeredBoxelSystem>();
        _layerController = FindObjectOfType<LayerController>();
        _cMeshVoxelizerWindow = FindObjectOfType<C_MeshVoxelizerWindow>();
        _modelScaling = FindObjectOfType<ModelScaling>();
    }

    public void GeneratePlayModel()
    {
        Debug.Log("Scaling");
        _modelScaling.CalcModelScale();
        Debug.Log("Voxelize");
        _cMeshVoxelizerWindow.Voxelize();
        Debug.Log("Layering");
        _layeredBoxelSystem.DoLayering();
        //Debug.Log("AdvanceFloor");
        //_layerController.AdvanceFloor();
    }
}
