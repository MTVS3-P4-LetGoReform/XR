using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerManager : MonoBehaviour
{
   public static RunnerManager Instance { get; private set; }
   
   public NetworkRunner networkRunnerPrefab;
   public NetworkObject playerPrefab;
   
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
      await JoinPublicSession(); 
   }

   public async UniTask RunnerStart(StartGameArgs args,int sceneIndex = -1)
   {
      if (_runner == null)
      {
         InstantiateRunner();
      }

      var result = await _runner.StartGame(args);
      if (result.Ok)
      {
         Debug.Log($"세션이름: '{args.SessionName}'이 만들어졌습니다.");

         if (sceneIndex >= 0)
         {
            var loadSceneTask = SceneManager.LoadSceneAsync(sceneIndex);  // 씬 로딩
            await loadSceneTask;
         }
         
         await _runner.SpawnAsync(playerPrefab);
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
         InstantiateRunner();
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
      var sceneInfo = new NetworkSceneInfo();
      sceneInfo.AddSceneRef(SceneRef.FromIndex(1)); // MainScene
      var startArgs = new StartGameArgs
      {
         GameMode = GameMode.Shared,
         //Scene = sceneInfo,
         SessionName = "공용 세션"
      };
            
      await ShutdownRunner();
      await RunnerStart(startArgs);
   }

   private void InstantiateRunner()
   {
      _runner = Instantiate(networkRunnerPrefab);
      _runner.AddCallbacks(new NetworkCallbackHandler());
   }
}
