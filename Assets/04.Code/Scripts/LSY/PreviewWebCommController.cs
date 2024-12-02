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
        if (_debugModeData.DebugMode == true)
        {
            for (int i = 0; i < regenBtns.Count; i++)
            {
                int index = i;
                regenBtns[i].onClick.AddListener(() => DoPreviewRegen(targetImages[index]));
            }
            
            imageGenBtn.onClick.AddListener(DoPreviewImageGen);

        }
    }

    public void DoPreviewRegen(Image targetImage)
    {
        StartCoroutine(ImageRegenLoading(targetImage));
        
    }

    public void DoPreviewImageGen()
    {
        // _webApiData.ImageName = "MainMockDataImage.png";
        // _webApiData.ModelName = "MainMockDataModel.glb";
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

    IEnumerator ImageRegenLoading(Image targetImage)
    {
        _webCommManager.ActiveModelCommLoading();
        yield return new WaitForSeconds(1f);
        _webCommManager.DeactiveModelCommLoading();
        targetImage.sprite = regenImage;
    }
}
