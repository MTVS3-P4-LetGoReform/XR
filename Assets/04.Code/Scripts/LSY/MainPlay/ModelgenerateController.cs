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
    private Material rootMat;
    private Renderer rootRenderer;
    public Material CompletionMat;
    public Material orgMat;
    private float transparency = 0.5f;

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
        ChangeLayertoChild(modelObject, "OrgModel");
        //_layeredBoxelSystem.DeactivateAll();
    }

    // 한 층 올리기
    public void AdvanceLayer()
    {
        _layerController.AdvanceFloor();
    }

    // 오브젝트 및 오브젝트 자식 레이어 변경

    public void ChangeLayertoChild(GameObject root, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        root.layer = layer;
        ChangeLayerRecursively(root, layer);
    }
    public void ChangeLayerRecursively(GameObject root, int layer)
    {
        rootRenderer = root.GetComponent<Renderer>();
        if(rootRenderer != null){
            if (orgMat == null)
            {
                orgMat = rootRenderer.material;
            }
            rootRenderer.material = CompletionMat;
            //SetMaterialToTransparent(rootMat);
            //Color color = rootMat.color;
            //color.a = transparency; // 투명도 설정
            //rootMat.color = color;
        }
        root.layer = layer;
        foreach (Transform child in root.transform)
        {
            if (child != null)
            {
                ChangeLayerRecursively(child.gameObject, layer);
            }
        }

    }

    // private void SetMaterialToTransparent(Material material)
    // {
    //     material.SetFloat("_AlphaMode", 1); // 0: Opaque, 1: Blend
    //     Debug.Log($"ModelGenController : AlphaMode : {material.GetFloat("_AlphaMode")}");
    //     material.SetOverrideTag("RenderType", "Transparent");
    //     Debug.Log($"ModelGenController : RenderType : {material.GetFloat("RenderType")}");
    //     material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    //     Debug.Log($"ModelGenController : SrcBlend : {material.GetInt("_SrcBlend")}");
    //     material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    //     Debug.Log($"ModelGenController : DstBlend : {material.GetInt("_DstBlend")}");
    //     material.SetInt("_ZWrite", 0);
    //     Debug.Log($"ModelGenController : ZWrite : {material.GetInt("_ZWrite")}");
    //     material.renderQueue = 3000;
    //     Debug.Log($"ModelGenController : renderQueue : {material.renderQueue}");
    // }
}
