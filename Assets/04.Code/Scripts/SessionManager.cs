using System;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SessionManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner networkRunnerPrefab;
    public GameObject sessionPrefab;
    public Transform sessionListParent;
    public Button refreshButton; 
    
    public Canvas createRoomCanvas;
    public Button createSessionButton; 
   
    public TMP_InputField sessionNameInput;
    public TMP_InputField sessionIntroductionInput;
    public Button backButton;

    private List<SessionInfo> currentSessions = new List<SessionInfo>();
    private string _session;
    private NetworkRunner _runner;
    private async void Start()
    {
        _runner = Instantiate(networkRunnerPrefab);
        _runner.AddCallbacks(this);
        await JoinPublicSession(_runner);
        //refreshButton.onClick.AddListener(async () => await JoinPublicSession(networkRunner));
        createSessionButton.onClick.AddListener(CreateSession);
        backButton.onClick.AddListener(CanvasOnOff);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CanvasOnOff();
        }
    }
    
    private static async UniTask JoinPublicSession(NetworkRunner runner)
    {
        var arg = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "PublicSession",
        };
        var result = await runner.StartGame(arg);
        if (result.Ok)
        {
            Debug.Log("공용 세션에 입장했습니다.");
        }
        else
        {
            Debug.LogWarning($"세션 입장에 실패했습니다. : {result.ShutdownReason}");
        }
    }
    
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (Transform child in sessionListParent)
        {
            Destroy(child.gameObject);
        }
        
        currentSessions = sessionList;
        foreach (var session in sessionList)
        {
            if (session.Name != "PublicSession")
            {
                string description = "";
                if (session.Properties.TryGetValue("description", out var sessionDescription))
                {
                    description = sessionDescription;
                }
                Debug.Log("결괏값: " + description);
                
                GameObject sessionButton = Instantiate(sessionPrefab, sessionListParent);
                sessionButton.GetComponentInChildren<TMP_Text>().text = 
                    session.Name + $"<br>{description}<br>{session.PlayerCount} / {session.MaxPlayers}";
                Button button = sessionButton.GetComponent<Button>();
                button.onClick.AddListener(() => OnJoinSession(session));
            }
        }
    }

    public void CanvasOnOff()
    { ;
        bool canvasOff = createRoomCanvas.enabled;
        if (canvasOff)
        {
            createRoomCanvas.enabled = false;
        }
        else
        {
            createRoomCanvas.enabled = true;
        }
    }
    

    // 세션 생성 메서드
    private async void CreateSession()
    {
        string sessionName = sessionNameInput.text;

        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("세션이름이 비어있습니다.");
            return;
        }
        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(3));

        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 4,
            Scene = sceneInfo,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "description", sessionIntroductionInput.text }
            }
        };
        Debug.Log(startArgs);
        
        await _runner.Shutdown();
        
        var networkRunner = Instantiate(networkRunnerPrefab);
        var result = await networkRunner.StartGame(startArgs);
        
        if (result.Ok)
        {
            Debug.Log($"세션이름: '{sessionName}'이 만들어졌습니다. {sceneInfo}을 불러옵니다.");
        }
        else
        {
            Debug.LogError($"세션 생성에 실패했습니다.: {result.ShutdownReason}");
        }
    }
    
    private async UniTask Shutdown(NetworkRunner runner)
    {
        await runner.Shutdown();
    }
    
    private void OnJoinSession(SessionInfo session)
    {
        Debug.Log($"Joining session: {session.Name}");
        var args = new StartGameArgs
        {
            GameMode = GameMode.Shared,  
            SessionName = session.Name
        };
        var runner = Instantiate(networkRunnerPrefab);
        runner.StartGame(args);
    }
    

    // INetworkRunnerCallbacks 인터페이스 구현 (필요한 부분만 구현)
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
}
