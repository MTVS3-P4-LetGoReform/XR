using System.Collections;
using UnityEngine;

public class WebCommManager : MonoBehaviour
{
    private WebApiData webApiData;

    public string prompt;
    public string[] genImagePathList;
    public int selectedImageIndex;

    private int regenCnt = 3;

    private const int GenImageNum = 3;
    private const int RegenImageNum = 1;

    // 초기 이미지 생성
    public void DoRequestImageGen()
    {
        StartCoroutine(RequestImageGen());
    }
    
    // 이미지 재생성
    public void DoRequestImageGen(int idx)
    {
        StartCoroutine(RequestImageGen(idx));
    }
    private IEnumerator RequestImageGen()
    { 
        ImageGen _imageGen = new ImageGen(webApiData);

       yield return StartCoroutine(_imageGen.RequestImageGen(prompt, GenImageNum));
       genImagePathList = _imageGen._imageGenRes.filenames;
    }
    
    private IEnumerator RequestImageGen(int idx)
    { 
        ImageGen _imageGen = new ImageGen(webApiData);

        yield return StartCoroutine(_imageGen.RequestImageGen(prompt, RegenImageNum));
        genImagePathList[idx] = _imageGen._imageGenRes.filenames[idx];
    }
    
    
}