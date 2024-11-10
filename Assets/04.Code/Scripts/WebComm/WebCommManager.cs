using System.Collections;
using System.Collections.Generic;
using System.IO;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WebCommManager : MonoBehaviour
{
    public WebApiData webApiData;

    public TMP_Text promptInput;
    public List<Image> imageRenderers;
    
    private string prompt;
    public string[] genImageNameList;
    public int selectedImageIndex;
    public string modelName;

    private int regenCnt = 3;

    private const int GenImageNum = 3;
    private const int RegenImageNum = 1;

    // 초기 이미지 생성
    public void DoImageGenDown()
    {
        prompt = promptInput.text;
        StartCoroutine(ImageGenDown());
    }
    private IEnumerator ImageGenDown()
    { 
        ImageGen _imageGen = new ImageGen(webApiData);

        yield return StartCoroutine(_imageGen.RequestImageGen(prompt, GenImageNum, FirebaseAuthManager.Instance.UserId));
        genImageNameList = _imageGen._imageGenRes.filenames;

        ImageDownload _imageDownload = new ImageDownload(webApiData);
        for (int i = 0; i < GenImageNum; i++)
        {
            yield return StartCoroutine(_imageDownload.DownloadImage(genImageNameList[i]));
            ConvertSpriteFromPNG(imageRenderers[i], genImageNameList[i]);
        }
    }
    
    // 이미지 재생성
    public void DoRequestImageRegen()
    {
        StartCoroutine(RequestImageGen(selectedImageIndex));
    }
    private IEnumerator RequestImageGen(int idx)
    { 
        ImageGen _imageGen = new ImageGen(webApiData);

        yield return StartCoroutine(_imageGen.RequestImageGen(prompt, RegenImageNum, FirebaseAuthManager.Instance.UserId));
        genImageNameList[idx] = _imageGen._imageGenRes.filenames[0];
        ImageDownload _imageDownload = new ImageDownload(webApiData);
        yield return StartCoroutine(_imageDownload.DownloadImage(genImageNameList[idx]));
        ConvertSpriteFromPNG(imageRenderers[idx], genImageNameList[idx]);
    }

    public void SetIndex0()
    {
        selectedImageIndex = 0;
    }
    public void SetIndex1()
    {
        selectedImageIndex = 1;
    }
    public void SetIndex2()
    {
        selectedImageIndex = 2;
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
    // 모델 생성
    public void DoModelGenDown()
    {
        StartCoroutine(ModelGenDown());
    }
    public IEnumerator ModelGenDown()
    {
        ModelGen _modelGen = new ModelGen(webApiData);
        yield return StartCoroutine(_modelGen.RequestModelGen(genImageNameList[selectedImageIndex], webApiData.ModelId));
        Debug.Log(_modelGen._modelGenRes.filename);
        modelName = _modelGen._modelGenRes.filename;
        ModelDown _modelDown = new ModelDown(webApiData);
        yield return StartCoroutine(_modelDown.DownloadGLBFile(modelName));
        _modelDown.LoadAndInstantiateGLB(modelName);
        //FIXME : 가이드라인 생성으로 추후 변경
    }
}