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

    private const int DelayMilliseconds = 10000;

    private async void Start()
    {
        if (webApiData == null || debugModeData == null)
        {
            Debug.LogError("WebApiData 또는 DebugModeData가 초기화되지 않았습니다.");
            return;
        }
        
        masterRewordButton.onClick.AddListener(MasterRewordHandler);
        userRewordButton.onClick.AddListener(UserRewordHandler);
        
        _storageDatabase = new StorageDatabase(webApiData, debugModeData);

           
        await LoadSessionInfo();
        
        GameStateManager.Instance.Complete += SetReword;
    }

    private async void MasterRewordHandler()
    {
        await MasterReword();
    }

    private async void UserRewordHandler()
    {
        await UserReword();
    }

    private async UniTask LoadSessionInfo()
    {
        await UniTask.Delay(DelayMilliseconds);

        _sessionInfo = RunnerManager.Instance.runner.SessionInfo;
        if (_sessionInfo == null)
        {
            Debug.LogError("SessionInfo가 초기화되지 않았습니다.");
        }
    }

    private void SetReword(bool reword)
    {
        _sessionInfo = RunnerManager.Instance.runner.SessionInfo;
        if (_sessionInfo == null)
        {
            Debug.LogError("SessionInfo가 초기화되지 않았습니다.");
            return;
        }

        string imageName = GetImage(_sessionInfo);
        LoadImage(imageName).Forget(); // 비동기 메서드를 기다릴 필요가 없으면 Forget 사용
    }

    private async UniTask MasterReword()
    {
        try
        {
            var userId = UserData.Instance.UserId;
            Cursor.lockState = CursorLockMode.Locked;

            _sessionInfo = RunnerManager.Instance.runner.SessionInfo;
            if (_sessionInfo == null)
            {
                Debug.LogError("SessionInfo가 초기화되지 않았습니다.");
                return;
            }

            string modelId = GetModelId(_sessionInfo);
            RealtimeDatabase.CopyModelToUser(userId, modelId);

            Debug.Log("보상 획득: 스태츄");

            await RunnerManager.Instance.JoinPublicSession();
        }
        catch (Exception ex)
        {
            Debug.LogError($"MasterReword 처리 중 오류 발생: {ex.Message}");
        }
    }

    private string GetModelId(SessionInfo session)
    {
        string modelId = "";
        if (session.Properties.TryGetValue("ModelId", out var sessionModelId))
        {
            modelId = sessionModelId;
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 모델 ID를 불러오지 못했습니다.");
        }

        Debug.Log($"결괏값: {modelId}");
        return modelId;
    }

    private string GetImage(SessionInfo session)
    {
        string imageName = "";
        if (session.Properties.TryGetValue("ImageName", out var sessionImageName))
        {
            imageName = sessionImageName;
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 이미지를 불러오지 못했습니다.");
        }

        return imageName;
    }

    private async UniTask LoadImage(string imageName)
    {
        try
        {
            string url = Path.Combine(Application.persistentDataPath,"images",imageName);

            // WebApiData 업데이트 (필요한 경우 로컬 변수로 처리)
            webApiData.ImageName = imageName;

            // 이미지 다운로드
            await _storageDatabase.DownImage(webApiData.ImageName);

            await ReadImage(url);
            if (!File.Exists(url))
            {
                Debug.LogWarning($"파일이 존재하지 않습니다: {url}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"이미지 로드 중 오류 발생: {ex.Message}");
        }
        
    }

    private async UniTask ReadImage(string url)
    {
        byte[] fileData = await File.ReadAllBytesAsync(url);
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
                Debug.LogWarning("Statue가 null입니다. 스프라이트를 설정할 수 없습니다.");
            }
        }
        
    }
    
    private async UniTask UserReword()
    {
        try
        {
            Debug.Log("보상 획득: 크레딧");

            Cursor.lockState = CursorLockMode.Locked;

            // 크레딧 얻는 로직 추가 필요

            await RunnerManager.Instance.JoinPublicSession();
        }
        catch (Exception ex)
        {
            Debug.LogError($"UserReword 처리 중 오류 발생: {ex.Message}");
        }
        
    }
}
