using UnityEngine;
using UnityEngine.UI;

public class TempDecoMarket : MonoBehaviour
{
   public GameObject ChriTree;
   public Canvas bpShopCanvas;
   public Canvas userInfoCanvas;
   public Image imagePurchaseBG;
   public Image imageCompPurchaseBG;


   public void tempTreeButtonTrueOnClick()
   {
      
   }

   public void bPShopButtonTrueOnClick()
   {
      bpShopCanvas.gameObject.SetActive(true);
      userInfoCanvas.gameObject.SetActive(false);
   }

   public void bpShopCloseButtonOnClick()
   {
      bpShopCanvas.gameObject.SetActive(false);
      userInfoCanvas.gameObject.SetActive(true);
   }

   public void bpShopPurchaseButtonOnClick()
   {
      imagePurchaseBG.gameObject.SetActive(true);
      
      ChriTree.gameObject.SetActive(true);
   }
   
   public void PurchaseBgPurchaseButtonOnClick()
   {
      imageCompPurchaseBG.gameObject.SetActive(true);
      imagePurchaseBG.gameObject.SetActive(false);
   }

   public void PurchaseBgCancelButtonOnClick()
   {
      imagePurchaseBG.gameObject.SetActive(false);
   }

   public void PurchaseCompButtonOnClick()
   {
      imageCompPurchaseBG.gameObject.SetActive(false);
   }
}
