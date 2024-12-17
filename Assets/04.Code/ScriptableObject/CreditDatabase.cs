using UnityEngine;

[CreateAssetMenu(fileName = "CreditDatabase", menuName = "Scriptable Objects/CreditDatabase")]
public class CreditDatabase : ScriptableObject
{
    [SerializeField] 
    private int creditCount;
    
    public int CreditCount
    { 
        set { creditCount = value;}
        get { return creditCount; }
    }
    
    [SerializeField]
    private int dreamTicketCount;

    public int DreamTicketCount
    {
        set { dreamTicketCount = value;}
        get { return dreamTicketCount; }
    }
    
}
