using UnityEngine;
using UnityEngine.Networking;

public class ImageDownload
{
    public ImageDownReq _imageDownReq;
    private WebApiData webApiData;
    
    public ImageDownload(WebApiData webapi)
    {
        webApiData = webapi;
    }
    public ImageDownload()
    {
        _imageDownReq = new ImageDownReq();
    }

    public void DownloadImage(string imgpath)
    {
        _imageDownReq.filename = imgpath;

        UnityWebRequest request = new UnityWebRequest(webApiData.Baseurl + webApiData.ImageDownPoint, "POST");
        
        string jsonData = JsonUtility.ToJson(_imageDownReq)
    }
}        