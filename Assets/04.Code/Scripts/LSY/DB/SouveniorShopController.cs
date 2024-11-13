using UnityEngine;

public class SouveniorShopController:MonoBehaviour
{
    public WebApiData webApiData;
    
    public void PurchasePicachu()
    {
        webApiData.ImageName = "image03.png";
        webApiData.ModelName = "image03.glb";
    }
    
}