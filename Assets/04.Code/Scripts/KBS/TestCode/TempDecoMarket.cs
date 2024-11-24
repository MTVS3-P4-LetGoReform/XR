using UnityEngine;
using UnityEngine.UI;

public class TempDecoMarket : MonoBehaviour
{
   public Button tempTreeButton;
   public Canvas bpShopCanvas;
   public Canvas userInfoCanvas;


   public void tempTreeButtonTrueOnClick()
   {
      tempTreeButton.gameObject.SetActive(true);
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
}
