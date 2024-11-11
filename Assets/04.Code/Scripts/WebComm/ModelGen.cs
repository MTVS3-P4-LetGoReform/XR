using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ModelGen
{
    public ModelGenReq _modelGenReq;
    public ModelGenRes _modelGenRes;
    private WebApiData webApiData;

    public ModelGen(WebApiData webapi)
    {
        _modelGenReq = new ModelGenReq();
        _modelGenRes = new ModelGenRes();
        webApiData = webapi;
    }

    public IEnumerator RequestModelGen(string fname, string modelId)
    {
        _modelGenReq.image_filename = fname;
        _modelGenReq.model_id = modelId;
        webApiData.ModelId = modelId;
        UnityWebRequest request = new UnityWebRequest( webApiData.Baseurl+webApiData.ModelGenPoint, "POST");
        string jsonData = JsonUtility.ToJson(_modelGenReq);
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
            _modelGenRes = JsonUtility.FromJson<ModelGenRes>(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
