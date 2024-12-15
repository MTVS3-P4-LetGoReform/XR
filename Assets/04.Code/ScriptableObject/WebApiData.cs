using GLTFast;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "WebApiScriptableObject", menuName = "Scriptable Objects/WebApiScriptableObject", order = 1)]
public class WebApiData : ScriptableObject
{
    [SerializeField] private string userId;

    public string UserId
    {
        set { userId = value;}
        get { return userId; }
    }
    [SerializeField] private string modelId;
    public string ModelId
    {
        set { modelId = value;}
        get { return modelId; }
    }

    [SerializeField] private string imageName;

    public string ImageName
    {
        get { return imageName; }
        set { imageName = value; }
    }
    
    [SerializeField] private string modelName;

    public string ModelName
    {
        set { modelName = value; }
        get { return modelName; }
    }
    [SerializeField] 
    private string baseUrl;

    public string Baseurl
    {
        get { return baseUrl;}
    }

    [SerializeField] private string tempBaseUrl;

    public string TempBaseurl
    {
        get { return tempBaseUrl; }
    }

    [SerializeField] 
    private string imageGenPoint;

    public string ImageGenPoint
    {
        get { return imageGenPoint;}
    }
    
    [SerializeField] 
    private string imageDownPoint;

    public string ImageDownPoint
    {
        get { return imageDownPoint; }
    }
    
    [SerializeField] 
    private string modelGenPoint;

    public string ModelGenPoint
    {
        get { return modelGenPoint; }
    }
    
    [SerializeField] 
    private string modelDownPoint;

    public string ModelDownPoint
    {
        get { return modelDownPoint; }
    }

    [SerializeField] 
    private string imageSketchGenPoint;

    public string ImageSketchGenPoint
    {
        get { return imageSketchGenPoint; }
    }

    [SerializeField] private string inpaintGenPoint;

    public string InpaintGenPoint
    {
        get { return inpaintGenPoint; }
    }
    [SerializeField] private string storageBaseUrl;

    public string StorageBaseUrl
    {
        get { return storageBaseUrl; }
    }

    [SerializeField] private string storageModelsPoint;

    public string StorageModelsPoint
    {
        get { return storageModelsPoint; }
    }

    [SerializeField] private string tempModelName;

    public string TempModelName
    {
        get { return tempModelName; }
    }
    
    [SerializeField] private Sprite modelSprite;

    public Sprite ModelSprite
    {
        set { modelSprite = value; }
        get { return modelSprite; }
    }

    [SerializeField] private GltfImport modelGltfImport;
    public GltfImport ModelGltfImport
    {
        set {
            modelGltfImport = value;
        }
        get {
            return modelGltfImport;
        }
    }
}
