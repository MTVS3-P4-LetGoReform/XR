using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class ImageRegen
{
    public ImageRegenReq _imageRegenReq;
    public ImageRegenRes _imageRegenRes;
    
    public WebApiData webApiData;
    public ImageRegen(WebApiData webapi)
    {
        _imageRegenReq = new ImageRegenReq();
        _imageRegenRes = new ImageRegenRes();
        webApiData = webapi;
    }
    
    public IEnumerator RequestImageRegen(string pmpt, int num, string creatorId, string modelId)
    {
        _imageRegenReq.prompt = pmpt;
        _imageRegenReq.batch = num;
        _imageRegenReq.creator_id = creatorId;
        _imageRegenReq.model_id = modelId;
        
        // // JSON 데이터 생성
        // string jsonData = "{\"filename\":\"" + fileName + "\"}";

        // UnityWebRequest Post요청 생성
        Debug.Log(webApiData.Baseurl+webApiData.ImageGenPoint);
        UnityWebRequest request = new UnityWebRequest( webApiData.Baseurl+webApiData.ImageGenPoint, "POST");
        // 요청 데이터 직렬화
        string jsonData = JsonUtility.ToJson(_imageRegenReq);
        // 문자열 데이터를 UTF-8바이트로 변환(Post 요청 본문 형식으로 변환)
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        
        // 데이터 요청 본문에 추가
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        // 서버 응답 데이터를 메모리에 저장하기 위한 선언
        request.downloadHandler = new DownloadHandlerBuffer();
        // 요청 헤더 설정
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();
        
        // 응답 체크
        if (request.result == UnityWebRequest.Result.Success)
        {
            _imageRegenRes = JsonUtility.FromJson<ImageRegenRes>(request.downloadHandler.text);
            Debug.Log($"ImageGen : {_imageRegenRes.id}");
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}