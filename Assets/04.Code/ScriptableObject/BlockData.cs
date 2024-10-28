using UnityEngine;

[CreateAssetMenu(fileName = "BlockScriptableObject", menuName = "Scriptable Objects/BlockScriptableObject", order = 0)]
public class BlockData : ScriptableObject
{
    [SerializeField] private int blockNumber;

    public int BlockNumber
    {
        get;
        set;
    }
}
