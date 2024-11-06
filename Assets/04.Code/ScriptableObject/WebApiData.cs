using UnityEngine;

[CreateAssetMenu(fileName = "WebApiScriptableObject", menuName = "Scriptable Objects/WebApiScriptableObject", order = 1)]
public class WebApiData : ScriptableObject
{
    [SerializeField] 
    private string baseUrl;

    public string Baseurl
    {
        get { return baseUrl;}
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
}
