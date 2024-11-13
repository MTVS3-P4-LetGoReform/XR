using UnityEngine;

public class SouveniorShopController:MonoBehaviour
{
    public WebApiData webApiData;
    public StatueInventoryDB _statueInventoryDB;

    public void Start()
    {
        _statueInventoryDB = GameObject.FindObjectOfType<StatueInventoryDB>();
    }
    public void PurchasePicachu()
    {
        webApiData.ImageName = "image03.png";
        webApiData.ModelName = "image03.glb";
        _statueInventoryDB.SetGenImage();
    }
    
}