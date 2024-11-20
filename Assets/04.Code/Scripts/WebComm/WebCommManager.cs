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
    private string prompt;
    public string[] genImageNameList;
    public int selectedImageIndex;
    public string modelName;

    private int regenCnt = 3;
    public string modelId;
    private const int GenImageNum = 3;
    private const int RegenImageNum = 1;
    private string _userId;
    
    public Button createRoomStart;
    public Button ImageGenBtn;
    public Button ImageRengenBtn;
    private SessionUIManager _sessionUIManager;
    
    private bool isGenerating = false;
    public DebugModeData debugModeData;
    private NetworkRunner _networkRunner;
    private void Start()
    {
        if (UserData.Instance ==null)
            return;
        _userId = UserData.Instance.UserId;
        _sessionUIManager = FindObjectOfType<SessionUIManager>();
        createRoomStart.onClick.AddListener(DoModelGenDown);
        ImageGenBtn.onClick.AddListener(DoImageGenDown);
        ImageRengenBtn.onClick.AddListener(DoImageRegen);
        
        genImageList[0].GetComponent<Button>().onClick.AddListener(SetIndex0);
        genImageList[1].GetComponent<Button>().onClick.AddListener(SetIndex1);
        genImageList[2].GetComponent<Button>().onClick.AddListener(SetIndex2);
    }

    // 초기 이미지 생성
    public void DoImageGenDown()
    {
        // 디버그 모드 시 통신하지 않음.
        if (debugModeData.DebugMode == true)
        {
            return;
        }
        prompt = promptInput.text;
        StartCoroutine(ImageGenDown());
    }
    private IEnumerator ImageGenDown()
    {
        yield return null;
        ImageGen _imageGen = new ImageGen(webApiData);

        yield return StartCoroutine(_imageGen.RequestImageGen(prompt, GenImageNum,_userId ));
        modelId = _imageGen._imageGenRes.id;
        webApiData.ModelId = modelId;
        genImageNameList = _imageGen._imageGenRes.filenames;

        ImageDownload _imageDownload = new ImageDownload(webApiData);
        for (int i = 0; i < GenImageNum; i++)
        {
            yield return StartCoroutine(_imageDownload.DownloadImage(genImageNameList[i]));
            ConvertSpriteFromPNG(genImageList[i].GetComponent<Image>(), genImageNameList[i]);
        }
    }
    
    // 이미지 재생성
    public void DoImageRegen()
    {
        StartCoroutine(RequestImageGen(selectedImageIndex));
    }
    private IEnumerator RequestImageGen(int idx)
    { 
        ImageRegen _imageRegen = new ImageRegen(webApiData);

        yield return StartCoroutine(_imageRegen.RequestImageRegen(prompt, RegenImageNum, FirebaseAuthManager.Instance.UserId, webApiData.ModelId));
        genImageNameList[idx] = _imageRegen._imageRegenRes.filenames[0];
        ImageDownload _imageDownload = new ImageDownload(webApiData);
        yield return StartCoroutine(_imageDownload.DownloadImage(genImageNameList[idx]));
        ConvertSpriteFromPNG(genImageList[idx].GetComponent<Image>(), genImageNameList[idx]);
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
        Debug.Log(5);
        
        isGenerating = false;
        Debug.Log(6);
    }

    public IEnumerator ModelGenDown()
    {
        ModelGen _modelGen = new ModelGen(webApiData);
        
        if (debugModeData.DebugMode == false)
        {
            
            webApiData.ImageName = genImageNameList[selectedImageIndex];
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
            StorageDatabase _storageDatabase = new StorageDatabase(webApiData, debugModeData);
            _storageDatabase.DownModelPlaySession(_modelGen._modelGenRes.model_filename, _sessionUIManager)
                    .Forget();
                Debug.Log(4);
        }
        else
        {
            StorageDatabase _storageDatabase = new StorageDatabase(webApiData, debugModeData);
            _storageDatabase.DownModelPlaySession(webApiData.ModelName, _sessionUIManager).Forget();
        }

    }

    public void JoinWebComm(string filename)
    {
        StorageDatabase _storageDatabase = new StorageDatabase(webApiData, debugModeData);
        _storageDatabase.DownModel(filename);

    }
}