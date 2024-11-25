using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StorgeTest : MonoBehaviour
{
    public WebApiData webApiData;
    public DebugModeData debugModeData;
    private StorageDatabase _storageDatabase;

    private string _selecetImageName = "241112_204518_8962318021d9594b534a29d0358c8d9c_0.png";
    public Image image;
    private async void Start()
    {
        _storageDatabase = new StorageDatabase(webApiData, debugModeData);
        await RealtimeDatabase.InitializeFirebaseAsync();
        SetImage(_selecetImageName,image);
    }
    
    private string GetImageUrl(string imageName)
    {
        string url = Path.Combine(Application.persistentDataPath,"Images",imageName);
        webApiData.ImageName = imageName;
        return url;
    }
    
    private async void SetImage(string imageName, Image modelImage)
    {
        string url = Path.Combine(Application.persistentDataPath,"Images",imageName);
        Debug.Log("url : " + url);
        
        Debug.Log("다운로드 시작");
        await _storageDatabase.DownLoadImage(imageName, url);
        UpdateImage(url,modelImage).Forget();
    } 
    
    private async UniTaskVoid UpdateImage(string url, Image targetImage)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("targetImage가 null입니다. 스프라이트를 설정할 수 없습니다.");
            return;
        }
        
        if (!File.Exists(url))
        {
            Debug.LogWarning("경로에 이미지 파일이 존재하지 않습니다.");
            return;
        }

        var req = UnityWebRequestTexture.GetTexture(url);
        await req.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(req);
        
        var sprite = Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        sprite.name = Path.GetFileName(url);
        targetImage.sprite = sprite;

        Debug.Log($"스프라이트 설정 완료: {sprite.name}");
    }
}
