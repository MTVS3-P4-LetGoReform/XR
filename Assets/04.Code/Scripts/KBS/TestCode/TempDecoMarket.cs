using UnityEngine;
using UnityEngine.UI;

public class TempDecoMarket : MonoBehaviour
{
   public Button tempTreeButton;
   public Canvas bpShopCanvas;


   public void tempTreeButtonTrueOnClick()
   {
      tempTreeButton.gameObject.SetActive(true);
   }

   public void bPShopButtonTrueOnClick()
   {
      bpShopCanvas.gameObject.SetActive(true);
   }
}
