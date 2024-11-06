using System.Collections;
using System.IO;
using System.Text;
using GLTFast.Schema;
using UnityEngine;
using UnityEngine.Networking;

public class ImageDownload
{
    public ImageDownReq _imageDownReq;
    private WebApiData webApiData;
    
    public ImageDownload(WebApiData webapi)
    {
        webApiData = webapi;
        _imageDownReq = new ImageDownReq();
    }
    public IEnumerator DownloadImage(string imgName)
    {
        if (string.IsNullOrEmpty(imgName))
        {
            Debug.LogError("ImageDownload: imgpath is null or empty.");
            yield break;
        }
        _imageDownReq.filename = imgName;

        UnityWebRequest request = new UnityWebRequest(webApiData.Baseurl + webApiData.ImageDownPoint, "POST");

        string jsonData = JsonUtility.ToJson(_imageDownReq);
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            //var tex = DownloadHandlerTexture.GetContent(request);
            // if (string.IsNullOrEmpty(tex.name))
            // {
            //     if (_imageDownReq.filename != null)
            //     {
            //         tex.name = Path.GetFileName(_imageDownReq.filename);
            //     }
            //     else
            //     {
            //         Debug.Log("ImageDownload : _imageDownReq.filename is null");
            //     }
            // }
            //
            // byte[] bytes = tex.EncodeToPNG();
            
            byte[] fileData = request.downloadHandler.data;

            // 이미지 폴더 경로 생성
            string folderPath = Path.Combine(Application.persistentDataPath, "Images");
            // 폴더가 없을 시 폴더 생성
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Folder created at: "+folderPath);
            }
            else
            {
                Debug.Log("Folder alread exits at: "+folderPath);
            }
            
            // 파일 경로 생성
            string filePath = Path.Combine(folderPath, _imageDownReq.filename);
            File.WriteAllBytes(filePath, fileData);
            Debug.Log("File written at: "+filePath);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}        