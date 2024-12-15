using System.Collections;
using System.IO;
using GLTFast;
using UnityEngine;
using UnityEngine.UI;

public class SouvenirMarketController : MonoBehaviour
{
    public Canvas canvasSouvenir;
    public Image imageGoodsPage;
    public Image imagePurchase;
    public Image imageEndPurchase;

    public Sprite hamoSprite;
    public GltfImport hamoGltfImport;
    public StatueInventoryController _statueInventoryController;

    public Button purchaseBtn;
    public void Start()
    {
        StartCoroutine(FindStatueInventoryController());
        purchaseBtn.onClick.AddListener(BuyToyPick);
    }

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
        Vector2 newVec = new Vector2(290, -200);
        imageEndPurchase.rectTransform.anchoredPosition = newVec;
        BuyToyPick();
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

    public async void BuyToyPick()
    {
        hamoGltfImport = new GltfImport();
        hamoGltfImport = await GltfLoader.LoadGLTF(Path.Combine(Application.persistentDataPath, "Models", "Souvenior", "hamo.glb"));
        _statueInventoryController.AddStatueToInven("id_m_0000", "tempImage0", "tempModel0", hamoSprite, hamoGltfImport, "tempcreatorId");
    }

    IEnumerator FindStatueInventoryController()
    {
        while (_statueInventoryController == null)
        {
            _statueInventoryController = FindObjectOfType<StatueInventoryController>();
            yield return null; 
        }
    }
}
