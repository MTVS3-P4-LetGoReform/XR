using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ImageSketchGen
{
    public ImageSketchGenReq imageSketchGenReq;
    public ImageSketchGenRes imageSketchGenRes;
    public WebApiData webApiData;
    public ImageSketchGen(WebApiData webapi)
    {
        imageSketchGenReq = new ImageSketchGenReq();
        imageSketchGenRes = new ImageSketchGenRes();
        webApiData = webapi;
    }
    
    public IEnumerator RequestImageGen(string base64Image, int num, string id)
    {
        imageSketchGenReq.image = base64Image;
        imageSketchGenReq.batch = num;
        imageSketchGenReq.creator_id = id;
        webApiData.UserId = id;
        
        // // JSON 데이터 생성
        // UnityWebRequest Post요청 생성
        Debug.Log(webApiData.Baseurl+webApiData.ImageGenPoint);
        UnityWebRequest request = new UnityWebRequest( webApiData.Baseurl+webApiData.ImageSketchGenPoint, "POST");
        // 요청 데이터 직렬화
        string jsonData = JsonUtility.ToJson(imageSketchGenReq);
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
            imageSketchGenRes = JsonUtility.FromJson<ImageSketchGenRes>(request.downloadHandler.text);
            //webApiData.ModelId = _imageGenRes.model_id;
            Debug.Log($"ImageGen : {imageSketchGenRes.id}");
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
