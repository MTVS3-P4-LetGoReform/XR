using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class RewordCanvas : MonoBehaviour
{
    public Canvas masterCanvas;
    public Button masterRewordButton;
    public Image statue;
    
    public Canvas userCanvas;
    public Button userRewordButton;

    [SerializeField]
    private WebApiData webApiData;
    private StorageDatabase _storageDatabase;
    private SessionInfo _sessionInfo;
    public DebugModeData debugModeData;
    private void Start()
    {
        masterRewordButton.onClick.AddListener(MasterReword);
        userRewordButton.onClick.AddListener(UserReword);
        _storageDatabase = new StorageDatabase(webApiData, debugModeData);
        GameStateManager.Instance.Complete += SetReword;
        Test().Forget();
    }


    private async UniTask Test()
    {   
        await UniTask.Delay(10000);
        Debug.Log("10초지남");
        _sessionInfo = RunnerManager.Instance.runner.SessionInfo;
        string imageName = GetImage(_sessionInfo);
        LoadImage(imageName);
    }
    
    private void SetReword(bool reword)
    {
        _sessionInfo = RunnerManager.Instance.runner.SessionInfo;
        string imageName = GetImage(_sessionInfo);
        LoadImage(imageName);
    }
    
    private async void MasterReword()
    {
        var userId = UserData.Instance.UserId;
        
        Cursor.lockState = CursorLockMode.Locked;
        
        _sessionInfo = RunnerManager.Instance.runner.SessionInfo;
        string modelId = GetModelId(_sessionInfo);
        
        
        RealtimeDatabase.CopyModelToUser(userId, modelId);
        Debug.Log("보상획득: 스태츄");
        await RunnerManager.Instance.JoinPublicSession();
    }

    private string GetModelId(SessionInfo session)
    {
        string ModelId = "";
        
        if (session.Properties.TryGetValue("ModelId", out var sessionDescription))
        {
            ModelId = sessionDescription;
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 이미지를 불러오지 못했습니다.");
        }
        Debug.Log("결괏값: " + ModelId);
        return ModelId;
    }
    
    private string GetImage(SessionInfo session)
    {
        string ImageName = "";
        
        if (session.Properties.TryGetValue("ImageName", out var sessionDescription))
        {
            ImageName = sessionDescription;
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 이미지를 불러오지 못했습니다.");
        }
        Debug.Log("결괏값: " + ImageName);
        return ImageName;
    }
    
    private async void LoadImage(string imageName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath);
        string url = Path.Combine(folderPath, imageName);
        webApiData.ImageName = imageName;

        await _storageDatabase.DownImage(webApiData.ImageName);

        if (File.Exists(url))
        {
            byte[] fileData = File.ReadAllBytes(url);
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(fileData))
            {
                if (statue != null)
                {
                    Sprite sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    statue.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("statue가 null입니다. 스프라이트를 설정할 수 없습니다.");
                }
            }
        }
    }
    
    private async void UserReword()
    {
        Debug.Log("보상획득: 크레딧");
        Cursor.lockState = CursorLockMode.Locked;
        //크레딧 얻는 로직 추가 필요
        await RunnerManager.Instance.JoinPublicSession();
    }
}
