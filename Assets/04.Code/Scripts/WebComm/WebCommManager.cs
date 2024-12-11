using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WebCommManager : MonoBehaviour
{
    public WebApiData webApiData;
    public TMP_Text promptInput;
    public List<GameObject> genImageList;
    public string prompt;
    public string[] genImageNameList;
    public int selectedImageIndex;
    public string modelName;

    private int regenCnt = 3;
    public string modelId;
    private const int GenImageNum = 3;
    private const int RegenImageNum = 1;
    private string _userId;
    
    public Button createRoomStart;
    public Button TxtImageGenBtn;
    public Button SketchImageGenBtn;
    public Button PngFileUploadBtn;
    public Button ImageRengenBtn;
    private SessionUIManager _sessionUIManager;

    public GameObject ImageCommLoadingObject;
    public GameObject ModelCommLoadingObject;
    
    private bool isGenerating = false;
    public DebugModeData debugModeData;
    private NetworkRunner _networkRunner;

    public Sprite mockSprite0;
    public Sprite mockSprite1;
    public Sprite mockSprite2;
    
    private void Start()
    {
        if (UserData.Instance ==null)
            return;
        _userId = UserData.Instance.UserId;
        _sessionUIManager = FindObjectOfType<SessionUIManager>();
        createRoomStart.onClick.AddListener(DoModelGenDown);
        //TxtImageGenBtn.onClick.AddListener(DoImageGenDown);
        SketchImageGenBtn.onClick.AddListener(DoSketchImageGenDown);
        PngFileUploadBtn.onClick.AddListener(GetSketchFileAndShow);
        ImageRengenBtn.onClick.AddListener(DoImageRegen);
        // FIX
        // genImageList[0].GetComponent<Button>().onClick.AddListener(SetIndex0);
        // genImageList[1].GetComponent<Button>().onClick.AddListener(SetIndex1);
        // genImageList[2].GetComponent<Button>().onClick.AddListener(SetIndex2);
    }

    // sketch 프롬프트 이미지 생성
    public void DoSketchImageGenDown()
    {
        /* FIXME : 디버그 모드 로직 추가 */
        if (debugModeData.DebugMode == true)
        {
            Debug.Log("DoSketchImageGen : DebugMode");
        }
        
        StartCoroutine(SketchImageGenDown());
    }

    private IEnumerator SketchImageGenDown()
    {
        /* FIXME : 디버그 모드 로직 추가 */
        if (debugModeData.DebugMode == true)
        {
            Debug.Log("SketchImageGen : DebugMode");
        }
        ActiveImageCommLoading();
        ImageSketchGen _sketchImageGen = new ImageSketchGen(webApiData);

        yield return StartCoroutine(_sketchImageGen.RequestImageGen(prompt, GenImageNum, _userId));
        modelId = _sketchImageGen.imageSketchGenRes.id;
        webApiData.ModelId = modelId;
        genImageNameList = _sketchImageGen.imageSketchGenRes.filenames;
        
        ImageDownload _imageDownload = new ImageDownload(webApiData);
        for (int i = 0; i < GenImageNum; i++)
        {
            yield return StartCoroutine(_imageDownload.DownloadImage(genImageNameList[i]));
            Image genImage = genImageList[i].GetComponent<Image>();
            ConvertSpriteFromPNG(genImage, genImageNameList[i]);
            Color color = genImage.color;
            color.a = 1f;
            genImage.color = color;
        }
        
        DeactiveImageCommLoading();


    }
    // 초기 이미지 생성
    public void DoImageGenDown()
    {
        Debug.Log("DoImageGenDown()");
        // 디버그 모드 시 통신하지 않음.
        if (debugModeData.DebugMode == true)
        {
            Debug.Log("DoImageGenDown() debugMode true");
            StartCoroutine(ShowImageCommLoading());
        }
        prompt = promptInput.text;
        StartCoroutine(ImageGenDown());
    }
    private IEnumerator ImageGenDown()
    {
        if(debugModeData.DebugMode == false){
            //yield return null;
            ActiveImageCommLoading();
            ImageGen _imageGen = new ImageGen(webApiData);

            yield return StartCoroutine(_imageGen.RequestImageGen(prompt, GenImageNum,_userId ));
            modelId = _imageGen._imageGenRes.id;
            webApiData.ModelId = modelId;
            genImageNameList = _imageGen._imageGenRes.filenames;

            ImageDownload _imageDownload = new ImageDownload(webApiData);
            for (int i = 0; i < GenImageNum; i++)
            {
                yield return StartCoroutine(_imageDownload.DownloadImage(genImageNameList[i]));
                Image genImage = genImageList[i].GetComponent<Image>();
                ConvertSpriteFromPNG(genImage, genImageNameList[i]);
                Color color = genImage.color;
                color.a = 1f;
                genImage.color = color;
            }
            DeactiveImageCommLoading();
        }
    }
    
    // 이미지 재생성
    public void DoImageRegen()
    {
        StartCoroutine(RequestImageGen(selectedImageIndex));
    }
    private IEnumerator RequestImageGen(int idx)
    {
        ActiveImageCommLoading();
        ImageRegen _imageRegen = new ImageRegen(webApiData);

        yield return StartCoroutine(_imageRegen.RequestImageRegen(prompt, RegenImageNum, FirebaseAuthManager.Instance.UserId, webApiData.ModelId));
        genImageNameList[idx] = _imageRegen._imageRegenRes.filenames[0];
        ImageDownload _imageDownload = new ImageDownload(webApiData);
        yield return StartCoroutine(_imageDownload.DownloadImage(genImageNameList[idx]));
        ConvertSpriteFromPNG(genImageList[idx].GetComponent<Image>(), genImageNameList[idx]);
        DeactiveImageCommLoading();
    }

    public void SetIndex0()
    {
        selectedImageIndex = 0;
        Debug.Log("index 0");
    }
    public void SetIndex1()
    {
        selectedImageIndex = 1;
        Debug.Log("index 1");
    }
    public void SetIndex2()
    {
        selectedImageIndex = 2;
        Debug.Log("index 2");
    }
    
    // Convert Png to Sprite and allocate Sprite to targetSpriteRenderer 
    public void ConvertSpriteFromPNG(Image targetImage, string fName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Images");
        string filePath = Path.Combine(folderPath, fName);

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            // x
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(fileData))
            {
                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = sprite;
            }
        }
    }

    public void DoModelGenDown()
    {
        if (!isGenerating)
        {
            isGenerating = true;
            StartCoroutine(ModelGenDownSceneLoad());
        }
    }
    
    public IEnumerator ModelGenDownSceneLoad()
    {
        yield return StartCoroutine(ModelGenDown());
        isGenerating = false;
    }

    public IEnumerator ModelGenDown()
    {
        ModelGen _modelGen = new ModelGen(webApiData);
        if (debugModeData.PreviewMode == true)
        {
            StorageDatabase.InitializStorageDatabase(webApiData, debugModeData);
            StorageDatabase.DownModelPlaySession(webApiData.ModelName, _sessionUIManager).Forget();
        }
        if (debugModeData.DebugMode == false)
        {
            ActiveModelCommLoading();
            webApiData.ImageName = genImageNameList[selectedImageIndex];
            webApiData.ModelSprite = genImageList[selectedImageIndex].GetComponent<Image>().sprite;
            yield return StartCoroutine(_modelGen.RequestModelGen(genImageNameList[selectedImageIndex], modelId));
            Debug.Log(_modelGen._modelGenRes.model_filename);
            if (_modelGen.request.result == UnityWebRequest.Result.Success)
            {
                modelName = _modelGen._modelGenRes.model_filename;
                webApiData.ModelName = _modelGen._modelGenRes.model_filename;
                // ModelDown _modelDown = new ModelDown(webApiData);
                // yield return StartCoroutine(_modelDown.DownloadGLBFile(modelName));
                // _modelDown.LoadAndInstantiateGLB(modelName);
                //FIXME : 가이드라인 생성으로 추후 변경

                
            }
            //StorageDatabase _storageDatabase = new StorageDatabase(webApiData, debugModeData);
            // TESTME : storagedatabase static 변경
            StorageDatabase.InitializStorageDatabase(webApiData, debugModeData);
            StorageDatabase.DownModelPlaySession(_modelGen._modelGenRes.model_filename, _sessionUIManager)
                    .Forget();
                Debug.Log(4);
            DeactiveModelCommLoading();
        }
        else
        {
            // TESTME : storagedatabase static 변경
            StorageDatabase.InitializStorageDatabase(webApiData, debugModeData);
            StorageDatabase.DownModelPlaySession(webApiData.ModelName, _sessionUIManager).Forget();
        }

    }

    public void JoinWebComm(string filename)
    {
        //StorageDatabase _storageDatabase = new StorageDatabase(webApiData, debugModeData);
        StorageDatabase.InitializStorageDatabase(webApiData, debugModeData);
        StorageDatabase.DownModel(filename);

    }

    public void ActiveImageCommLoading()
    {
        ImageCommLoadingObject.SetActive(true);
    }
    public void DeactiveImageCommLoading()
    {
        ImageCommLoadingObject.SetActive(false);
    }

    public void ActiveModelCommLoading()
    {
        ModelCommLoadingObject.SetActive(true);
    }
    public void DeactiveModelCommLoading()
    {
        ModelCommLoadingObject.SetActive(false);
    }

    IEnumerator ShowImageCommLoading()
    {
        Image image;
        Color color;
        ModelCommLoadingObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        ModelCommLoadingObject.SetActive(false);
        image = genImageList[0].GetComponent<Image>();
        image.sprite = mockSprite0;
        color = image.color;
        color.a = 1f;
        image.color = color;
        image = genImageList[1].GetComponent<Image>();
        image.sprite = mockSprite1;
        color = image.color;
        color.a = 1f;
        image.color = color;
        image = genImageList[2].GetComponent<Image>();
        image.sprite = mockSprite2;
        color = image.color;
        color.a = 1f;
        image.color = color;
    }

    public void GetSketchFileAndShow()
    {
        Stream stream = PngFileDialog.FileOpen();
        byte[] buffer = PngFileDialog.ConvertPngStreamToByte(stream);
        PngFileUploadBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(420f, 467f);
        PngFileUploadBtn.GetComponent<Image>().sprite = PngFileDialog.ConvertByteToSprite(buffer);
        prompt = PngFileDialog.ConvertByteTOBase64(buffer);
    }
}