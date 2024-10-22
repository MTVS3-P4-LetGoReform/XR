using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
   public static NetworkManager Instance { get; private set; }
   
   public NetworkRunner networkRunnerPrefab;
   
   private NetworkRunner _runner;

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(gameObject);
      }
   }

   private async void Start()
   {
      if (SceneManager.GetActiveScene().name == "FusionTest 1")
      {
         await JoinPublicSession();
      }
   }

   public async UniTask RunnerStart(StartGameArgs args)
   {
      if (_runner == null)
      {
         _runner = Instantiate(networkRunnerPrefab);
         _runner.AddCallbacks(this);
      }

      var result = await _runner.StartGame(args);
      if (result.Ok)
      {
         Debug.Log($"세션이름: '{args.SessionName}'이 만들어졌습니다.");
      }
      else
      {
         Debug.LogError($"세션 생성에 실패했습니다.: {result.ShutdownReason}");
      }
   }
   
   public async UniTask ShutdownRunner()
   {
      if (_runner != null && _runner.IsRunning)
      {
         await _runner.Shutdown();
         _runner = null;
      }
   }

   public async UniTask JoinLobby()
   {
      if (_runner == null)
      {
         _runner = Instantiate(networkRunnerPrefab);
         _runner.AddCallbacks(this);
      }

      var result = await _runner.JoinSessionLobby(SessionLobby.Shared);
      if (result.Ok)
      {
         Debug.Log("로비에 입장하였습니다.");
      }
      else
      {
         Debug.Log($"로비 입장에 실패하였습니다. : {result.ShutdownReason}");
      }
   }

   public async UniTask JoinPublicSession()
   {
      if (_runner == null)
      {
         _runner = Instantiate(networkRunnerPrefab);
         _runner.AddCallbacks(this);
      }
      
      var startArgs = new StartGameArgs
      {
         GameMode = GameMode.Shared,
         SessionName = "공용 세션",
      };
            
      await ShutdownRunner();
      await RunnerStart(startArgs);
   }
   
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("세션 목록 업데이트");
        SessionUIManager.Instance.UpdateSessionList(sessionList);
    }

    
    // 로딩화면 구현시 사용 예정
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    // INetworkRunnerCallbacks 구현 (필요한 부분만 구현)
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
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject networkObject, PlayerRef player) { }
}
