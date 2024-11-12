using UnityEngine;
using UnityEngine.UI;

public class SouvenirMarketController : MonoBehaviour
{
    public Canvas canvasSouvenir;
    public Image imageGoodsPage;
    public Image imagePurchase;
    public Image imageEndPurchase;
    

    public void ExitButtonOnClick()
    {
        canvasSouvenir.gameObject.SetActive(false);
    }

    public void StatueImgOnClick()
    {
        imageGoodsPage.gameObject.SetActive(true);
    }

    public void StatueImgClose()
    {
        imageGoodsPage.gameObject.SetActive(false);
    }

    public void PurchaseButtonOnClick()
    {
        imagePurchase.gameObject.SetActive(true);
    }

    public void ConfirmPurchaseButtonOnClick()
    {
        imageEndPurchase.gameObject.SetActive(true);
    }

    public void FinishPurchaseButtonOnClick()
    {
        imageEndPurchase.gameObject.SetActive(false);
        imagePurchase.gameObject.SetActive(false);
    }

    public void CancelPurchaseButtonOnClick()
    {
        imagePurchase.gameObject.SetActive(false);
    }
}
