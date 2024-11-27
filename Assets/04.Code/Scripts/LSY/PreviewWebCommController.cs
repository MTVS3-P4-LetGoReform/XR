using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PreviewWebCommController : MonoBehaviour
{
    public DebugModeData _debugModeData;
    public WebApiData _webApiData;
    
    public List<Button> regenBtns;
    public List<Image> targetImages;
    public Sprite regenImage;

    public Button imageGenBtn;

    public WebCommManager _webCommManager;
    
    void Start()
    {
        StartCoroutine(FindWebCommManager());
        if (_debugModeData.PreviewMode == true)
        {
            for (int i = 0; i < regenBtns.Count; i++)
            {
                regenBtns[i].onClick.AddListener(() => DoPreviewRegen(targetImages[i]));
            }
            
            imageGenBtn.onClick.AddListener(DoPreviewImageGen);

        }
    }

    public void DoPreviewRegen(Image targetImage)
    {
        targetImage.sprite = regenImage;
    }

    public void DoPreviewImageGen()
    {
        _webApiData.ImageName = Path.Combine("MockData","MainMockDataImage.png");
        _webApiData.ModelName = Path.Combine("MockData", "MainMockDataModel.glb");
        _webCommManager.DoImageGenDown();
    }

    IEnumerator FindWebCommManager()
    {
        while (_webCommManager == null)
        {
            _webCommManager = FindObjectOfType<WebCommManager>();
            yield return null;
        }
    }

}
