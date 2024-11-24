using UnityEngine;

[CreateAssetMenu(fileName = "CreditDatabase", menuName = "Scriptable Objects/CreditDatabase")]
public class CreditDatabase : ScriptableObject
{
    [SerializeField] 
    private int creditCount;

    public int CreditCount
    {
        get;

        set;
    }
    
}
