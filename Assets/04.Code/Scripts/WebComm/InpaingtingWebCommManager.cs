using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using GLTFast;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InpaingtingWebCommManager : MonoBehaviour
{
    public DebugModeData debugModeData;
    public WebApiData webApiData;
    private StatueInventoryController statueInventoryController;
    public PlacedToypickData placedToypickData;

    //public PlacementToypick _placementToypick;

    public TMP_Text hairPrompt;
    public TMP_Text clothesPrompt;

    private InpaintingGen _inpaintingGen;
    private ImageDownload _imageDownload;
    private ModelGen _modelGen;
    private ModelDown _modelDown;
    
    public Image previewImage;
    public Button transferBtn;
    public Button addBtn;

    public string modelName;

    public void Start()
    {
        placedToypickData = new PlacedToypickData("241214_211734_8f9526e3f0dbe383b718634b2455ced8_0.glb",
            "241214_211734_8f9526e3f0dbe383b718634b2455ced8_0.png", null, null);
        statueInventoryController = GameObject.FindObjectOfType<StatueInventoryController>();
        if (statueInventoryController == null)
        {
            Debug.LogWarning("Statue Inventory Controller not found");
        }
        
        transferBtn.onClick.AddListener(async () => await DoInpainting());
        addBtn.onClick.AddListener(async () => await DoTransferring());
    }
    public async UniTask DoInpainting()
    {
        await InpaintingGenImageDown();
        previewImage.sprite = SpriteConverter.ConvertFromPNG(_inpaintingGen._inpaintingRes.filenames);
    }

    public async UniTask DoTransferring()
    {
        await InpaintingModelGenDownAdd();
    }
    public async UniTask InpaintingGenImageDown()
    {
        // 인페인팅 생성
        _inpaintingGen = new InpaintingGen(webApiData);
        await _inpaintingGen.RequestInpaintingGen(placedToypickData.imageName, hairPrompt.text,
            clothesPrompt.text, "cRShguIcsSX1ctiiruERJpFVwQD2");

        _imageDownload = new ImageDownload(webApiData);
        await _imageDownload.DownloadImage(_inpaintingGen._inpaintingRes.filenames);
        

    }

    public async UniTask InpaintingModelGenDownAdd()
    {
        _modelGen = new ModelGen(webApiData);

        await _modelGen.RequestModelGen(_inpaintingGen._inpaintingRes.filenames, _inpaintingGen._inpaintingRes.id);
        // StorageDatabase 초기화
        StorageDatabase.InitializStorageDatabase(webApiData, debugModeData);
        Debug.Log($"InpaintingModelGenDownAdd : {_modelGen._modelGenRes.model_filename}");
        
        if (_modelGen.request.result == UnityWebRequest.Result.Success)
        {
            modelName = _modelGen._modelGenRes.model_filename;
            webApiData.ModelName = _modelGen._modelGenRes.model_filename;
            // ModelDown _modelDown = new ModelDown(webApiData);
            // yield return StartCoroutine(_modelDown.DownloadGLBFile(modelName));
            // _modelDown.LoadAndInstantiateGLB(modelName);
            //FIXME : 가이드라인 생성으로 추후 변경

                
        }
        
        // 모델 다운로드
        await StorageDatabase.DownModel(modelName);

        //string modelId = _inpaintingGen._inpaintingRes.id;
        // PNG 데이터를 Sprite로 변환
        Sprite sprite = SpriteConverter.ConvertFromPNG(_inpaintingGen._inpaintingRes.filenames);
        
        // GLTF 모델 로드
        string modelPath = Path.Combine(Application.persistentDataPath, "Models", modelName);
        GltfImport gltfImport = await GltfLoader.LoadGLTF(modelPath);
        
        statueInventoryController.AddStatueToInven(_inpaintingGen._inpaintingRes.id, sprite, gltfImport);
    }

}