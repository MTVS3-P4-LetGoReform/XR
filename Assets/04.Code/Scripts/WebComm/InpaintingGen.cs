using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

public class InpaintingGen
{
    public InpaintingReq _inpaintingReq;
    public InpaintingRes _inpaintingRes;
    public WebApiData webApiData;
    
    public InpaintingGen(WebApiData webapi)
    {
        _inpaintingReq = new InpaintingReq();
        _inpaintingRes = new InpaintingRes();
        webApiData = webapi;
    }

    public IEnumerator RequestInpaintingGen(string orgImageName, string hair_prompt, string inpainting_prompt, string creatorId)
    {
        _inpaintingReq.creator_id = creatorId;
        // reqest 데이터 입력
        _inpaintingReq.image_filename = orgImageName;
        
        // 헤어 프롬프트 여부 및 입력
        if (hair_prompt.IsNullOrEmpty())
        {
            _inpaintingReq.is_hair_change = false;
        }
        else
        {
            _inpaintingReq.is_hair_change = true;
            _inpaintingReq.hair_prompt = hair_prompt;
        }
        
        // 의상 프롬프트 여부 및 입력
        if (inpainting_prompt.IsNullOrEmpty())
        {
            _inpaintingReq.is_clothes_change = false;
        }
        else
        {
            _inpaintingReq.is_clothes_change = true;
            _inpaintingReq.clothes_prompt = inpainting_prompt;
        }
        
        // UnityWebRequest Post 요청 생성
        Debug.Log(webApiData.Baseurl + webApiData.InpaintGenPoint);
        UnityWebRequest request = new UnityWebRequest( webApiData.Baseurl+webApiData.InpaintGenPoint, "POST");
        // 요청 데이터 직렬화
        string jsonData = JsonUtility.ToJson(_inpaintingReq);
        Debug.Log($"jsonData : {jsonData}");
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
            _inpaintingRes = JsonUtility.FromJson<InpaintingRes>(request.downloadHandler.text);
            //webApiData.ModelId = _imageGenRes.model_id;
            Debug.Log($"inpaintingGen model id : {_inpaintingRes.id} / inpaintingGen image file name : {_inpaintingRes.filenames} ");
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
    
    
}