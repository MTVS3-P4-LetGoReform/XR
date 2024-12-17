using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestUserData : MonoBehaviour
{
    public CreditDatabase _creditDatabase;

    public List<TMP_Text> creditTxts;
    public List<TMP_Text> dreamTicketTxts;

    public List<Button> ImageGenBtns;
    public Button ToypickPurchaseBtn;
    public void Start()
    {
        
        for (int i = 0; i < creditTxts.Count; i++)
        {
            creditTxts[i].text = _creditDatabase.CreditCount.ToString();
        }
        for (int i = 0; i < dreamTicketTxts.Count; i++)
        {
            Debug.Log($"dream ticket: {_creditDatabase.DreamTicketCount}");
            dreamTicketTxts[i].text = $"(보유 횟수 : {_creditDatabase.DreamTicketCount})";
        }

        if (ImageGenBtns.Count > 0)
        {
            for (int i = 0; i < ImageGenBtns.Count; i++)
            {
                ImageGenBtns[i].onClick.AddListener(MinusDreamTicket);
            }
        }

        if (ToypickPurchaseBtn != null)
        {
            ToypickPurchaseBtn.onClick.AddListener(BuyToypick);
        }


    }
    public void BuyToypick()
    {
        _creditDatabase.CreditCount = _creditDatabase.CreditCount - 1000;
        for (int i = 0; i < creditTxts.Count; i++)
        {
            creditTxts[i].text = _creditDatabase.CreditCount.ToString();
        }
    }
    
    public void MinusDreamTicket()
    {
        _creditDatabase.DreamTicketCount = _creditDatabase.DreamTicketCount - 1;
        for (int i = 0; i < dreamTicketTxts.Count; i++)
        {
            dreamTicketTxts[i].text = $"(보유 횟수 : {_creditDatabase.DreamTicketCount})";
        }
    }

}
