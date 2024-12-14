using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using GLTFast;
using TMPro;
using UnityEngine;
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
    private ModelDown _modelDown;
    
    public Image previewImage;
    public Button transferBtn;
    public Button addBtn;

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
        await InpaintingModelDownAdd();
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

    public async UniTask InpaintingModelDownAdd()
    {
        // StorageDatabase 초기화
        StorageDatabase.InitializStorageDatabase(webApiData, debugModeData);
        
        // 모델 다운로드
        await StorageDatabase.DownModel(_inpaintingGen._inpaintingRes.id);

        string modelId = _inpaintingGen._inpaintingRes.id;
        // PNG 데이터를 Sprite로 변환
        Sprite sprite = SpriteConverter.ConvertFromPNG(_inpaintingGen._inpaintingRes.filenames);
        // Firebase에서 모델 정보 읽어오기
        var modelInfo = await RealtimeDatabase.ReadDataAsync<ModelInfo>($"user_land/{modelId}/create_3d_name");
        if (modelInfo == null)
        {
            Debug.LogError("InpaintingWebCommManager : ModelInfo is null");
        }
        
        // GLTF 모델 로드
        string modelPath = Path.Combine(Application.persistentDataPath, "Models", modelInfo.create_3d_name);
        GltfImport gltfImport = await GltfLoader.LoadGLTF(modelPath);
        
        statueInventoryController.AddStatueToInven(_inpaintingGen._inpaintingRes.id, sprite, gltfImport);
    }

}