using UnityEngine;

[CreateAssetMenu(fileName = "WebApiScriptableObject", menuName = "Scriptable Objects/WebApiScriptableObject", order = 1)]
public class WebApiData : ScriptableObject
{
    [SerializeField] 
    private string baseUrl;

    public string Baseurl
    {
        get;
    }

    [SerializeField] 
    private string imageGenPoint;

    public string ImageGenPoint
    {
        get;
    }
    
    [SerializeField] 
    private string imageDownPoint;

    public string ImageDownPoint
    {
        get;
    }
    
    [SerializeField] 
    private string objGenPoint;

    public string ObjGenPoint
    {
        get;
    }
    
    [SerializeField] 
    private string objDownPoint;

    public string ObjDownPoint
    {
        get;
    }
}
