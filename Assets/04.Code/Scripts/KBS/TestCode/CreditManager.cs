using TMPro;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public CreditDatabase creditDatabase;
    public TMP_Text textCredit;
    void Start()
    {
        creditDatabase.CreditCount = 15000;
    }

    // Update is called once per frame
    void Update()
    {
        textCredit.text = $"{creditDatabase.CreditCount}";
    }
}
