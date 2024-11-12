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
}
