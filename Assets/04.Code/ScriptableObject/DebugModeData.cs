using UnityEngine;

[CreateAssetMenu(fileName = "DebugModeScriptableObject", menuName = "Scriptable Objects/DebugModeScriptableObject", order = 3)]
public class DebugModeData : ScriptableObject
{
    [SerializeField] private bool debugMode;

    public bool DebugMode
    {
        set { debugMode = value;}
        get { return debugMode; }
    }

    [SerializeField] private bool previewMode;

    public bool PreviewMode
    {
        set { previewMode = value; }
        get { return previewMode; }
    }
}
