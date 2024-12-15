using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SessionUIManager : MonoBehaviour
{
    public static SessionUIManager Instance { get; private set; }

    //public StorageDatabase _storageDatabase;
    public TMP_InputField sessionNameInput;
    public TMP_InputField sessionPromptInput;
    public Button[] maxPlayerCountButton;
    
    public Transform sessionListParent;
    public GameObject sessionPrefab;
    
    public WebApiData webApiData;
    public WebCommManager _webCommManager;
    public DebugModeData _debugModeData;

    public GameObject SessionPopUpPrefab;
    public Transform popUpParent;
    
    private int currentPlayerCount;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // TESTME : storagedatabase static 변경
        
        StorageDatabase.InitializStorageDatabase(webApiData, _debugModeData);
        
        for (int i = 0; i < maxPlayerCountButton.Length; i++)
        {
            int playerCount = i + 1; 
            maxPlayerCountButton[i].onClick.AddListener(() => CheckPlayerCounts(playerCount));
        }
    }

    private void CheckPlayerCounts(int count)
    {
        Debug.Log(count);
        currentPlayerCount = count;
    }

    // 세션 목록 UI 업데이트
    public async UniTask UpdateSessionList(List<SessionInfo> sessionList)
    {
        // 기존 목록 삭제 같은 목록이 겹치는걸 방지함.
        foreach (Transform child in sessionListParent)
        {
            Destroy(child.gameObject);
        }

        // 세션 정보를 불러옴
        foreach (var session in sessionList)
        {
            if (session.Name == "공용 세션")
            {
                return;
            }

            
            /*
            string check = CheckSession(session);
            if (string.IsNullOrEmpty(check))
            {
                Debug.LogWarning("유효하지 않은 세션입니다.");
                return;
            }
            */


            // 현재 세션의 로컬 복사본 생성
            var currentSession = session;
            string url = GetImageUrl(session);
            
            // 목록 생성
            GameObject sessionButton = Instantiate(sessionPrefab, sessionListParent);
            RoomInfo roomInfo = sessionButton.GetComponent<RoomInfo>();
            Image targetImage = roomInfo.image;

            await UniTask.Yield();
            // TESTME : storagedatabase static 변경
            await StorageDatabase.DownImage(webApiData.ImageName);

            UpdateImage(url, targetImage).Forget();
            
            var popUpObject = Instantiate(SessionPopUpPrefab, popUpParent);
            var popUpInfo = popUpObject.GetComponent<SessionPopUpInfo>();
            var popUpImage = popUpInfo.image;
            
            UpdateImage(url, popUpImage).Forget();
            
            await UniTask.Yield();
            
            //info 할당
            popUpInfo.roomName.text = currentSession.Name;
            popUpInfo.count.text = $"{currentSession.PlayerCount}/{currentSession.MaxPlayers}";
            
            roomInfo.roomName.text = currentSession.Name;
            roomInfo.count.text = $"{currentSession.PlayerCount}/{currentSession.MaxPlayers}";

            Button roomInfoButton = roomInfo.button;
            roomInfoButton.onClick.AddListener(() => popUpObject.SetActive(true));
            
            Button popUpInfoButton = popUpInfo.button;
            popUpInfoButton.onClick.AddListener(() => JoinPlaySession(currentSession));
            
        }
    }


    private string CheckSession(SessionInfo session)
    {
        if (session.Properties.TryGetValue("UserId", out var id))
        {
            Debug.Log("UserId 찾음: " + id);
            return id;
        }
    
        Debug.Log("UserId를 찾을 수 없음");
        return null;
    }

    
    private string GetImageUrl(SessionInfo session)
    {
        string imageName = "";
        
        if (session.Properties.TryGetValue("ImageName", out var sessionDescription))
        {
            imageName = sessionDescription;
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 이미지를 불러오지 못했습니다.");
        }
        
        Debug.Log("결괏값: " + imageName);
        string url = Path.Combine(Application.persistentDataPath,"Images",imageName);
        webApiData.ImageName = imageName;
        return url;
    }

    private async UniTaskVoid UpdateImage(string url, Image targetImage)
    {
        if (_debugModeData.DebugMode)
        {
            Debug.Log("디버그 모드입니다. 기본 이미지를 노출합니다.");
            return;
        }
        
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

    
    public async void CreatePlaySession()
    {
        Debug.Log("SessionUIManager : CreatePlaySession()");
        string sessionName = sessionNameInput.text;
        Debug.Log("SessionUIManager : flag1");
        
        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("세션 이름이 비어있습니다.");
            return;
        }

        if (currentPlayerCount == 0)
        {
            Debug.LogWarning("인원수가 선택되지 않았습니다.");
            return;
        }
        
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = currentPlayerCount,
            //Scene = sceneInfo,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "Prompt", sessionPromptInput.text },
                { "ImageName", webApiData.ImageName },
                { "ModelId", webApiData.ModelId },
                { "ModelName", webApiData.ModelName }
            }
        };

        Debug.Log($"SessionUIManager : ModelId - {startArgs.SessionProperties["ModelId"]}");
        Debug.Log($"SessionUIManager : ImageName - {startArgs.SessionProperties["ImageName"]}");
        
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(startArgs,2);
    }
    
    private async void JoinPlaySession(SessionInfo session)
    {
        if (session.Properties.TryGetValue("ModelName", out var sessionDescription))
        {
            webApiData.ModelName = sessionDescription;
            _webCommManager.JoinWebComm(sessionDescription);
        }
        else
        {
            Debug.LogWarning($"{session.Name}: 모델을 불러오지 못했습니다.");
        }
        
        Debug.Log($"Joining session: {session.Name}");
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = session.Name,
            //Scene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(""))
        };

        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(startArgs,2);
    }
}