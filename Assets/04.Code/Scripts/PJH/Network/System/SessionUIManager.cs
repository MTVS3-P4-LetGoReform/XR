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
    
    public Transform sessionListParent;
    public GameObject sessionPrefab;
    
    
    public WebApiData webApiData;
    public WebCommManager _webCommManager;
    public DebugModeData _debugModeData;

    public GameObject SessionPopUpPrefab;
    public Transform popUpParent;

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
        /*create.onClick.AddListener(ActiveCreateRoom);
        join.onClick.AddListener(ActiveRoomList);
        
        roomListBack.onClick.AddListener(OffRoomList);
        
        createRoomBack.onClick.AddListener(OffCreateRoom);*/
        //createRoomRecreate.onClick.AddListener(ImageCraft);
        //createRoomStart.onClick.AddListener(CreatePlaySession);
        // TESTME : storagedatabase static 변경
        StorageDatabase.InitializStorageDatabase(webApiData, _debugModeData);
    }

    // private void ImageCraft()
    // {
    //     test.SetActive(true);
    // }
    //

    // 세션 목록 UI 업데이트
    public async void UpdateSessionList(List<SessionInfo> sessionList)
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

            string check = CheckSession(session);
            if (check != null)
            {
                return;
            }

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
            
            await UniTask.Yield();
            
            //info 할당
            popUpInfo.roomName.text = session.Name;
            popUpInfo.count.text = $"{session.PlayerCount}/{session.MaxPlayers}";
            
            roomInfo.roomName.text = session.Name;
            roomInfo.count.text = $"{session.PlayerCount}/{session.MaxPlayers}";

            Button roomInfoButton = roomInfo.button;
            roomInfoButton.onClick.AddListener(() => popUpObject.SetActive(true));
            
            Button popUpInfoButton = popUpInfo.button;
            popUpInfoButton.onClick.AddListener(() => JoinPlaySession(session));
            
        }
    }


    private string CheckSession(SessionInfo session)
    {
        var userId = "";
        if (session.Properties.TryGetValue("UserId", out var id))
        {
            userId = id;
        }
        else
        {
            userId = null;
        }
        Debug.Log("결괏값: " + userId);
        return userId;
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
        Debug.Log("SeissionUIManager : CreatePlaySession()");
        string sessionName = sessionNameInput.text;
        Debug.Log("SeissionUIManager : flag1");
        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("세션 이름이 비어있습니다.");
            return;
        }
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 4,
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
        string ModelName = "";
        if (session.Properties.TryGetValue("ModelName", out var sessionDescription))
        {
            ModelName = sessionDescription;
            webApiData.ModelName = sessionDescription;
            _webCommManager.JoinWebComm(ModelName);
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

    private void PopUp()
    {
        
    }
}