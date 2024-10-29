using Cysharp.Threading.Tasks;
using Fusion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RunnerManager : MonoBehaviour
{
   public static RunnerManager Instance { get; private set; }
   
   public NetworkRunner networkRunnerPrefab;
   public NetworkObject playerPrefab;
   public NetworkRunner runner;
   
   public Transform publicParkSpawnPoint;
   public Transform playSpawnPoint;
   public Transform personalSpawnPoint;
   
   private NetworkObject _spawnedPlayer;
   
   private Transform _currentSpawnPoint;

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
      if (runner == null)
      {
         InstantiateRunner();
      }

      var result = await runner.StartGame(args);
      if (result.Ok)
      {
         Debug.Log($"세션이름: '{args.SessionName}'이 만들어졌습니다.");

         if (sceneIndex >= 0)
         {
            var loadSceneTask = SceneManager.LoadSceneAsync(sceneIndex);  // 씬 로딩
            await loadSceneTask;
         }

         await PlayerSpawn();
      }
      else
      {
         Debug.LogError($"세션 생성에 실패했습니다.: {result.ShutdownReason}");
      }
   }
   
   private async UniTask PlayerSpawn()
   {
      switch (SceneManager.GetActiveScene().name)
      {
         case "Proto_PublicParkScene":
            _currentSpawnPoint = publicParkSpawnPoint;
            break;
         case "Proto_PlayScene":
            _currentSpawnPoint = playSpawnPoint;
            break;
         case "Proto_PersonalScene":
            _currentSpawnPoint = personalSpawnPoint;
            break;
      }
     
      var playerOp = runner.SpawnAsync(playerPrefab,_currentSpawnPoint.position,quaternion.identity);
      UniTask.WaitUntil(() => playerOp.Status == NetworkSpawnStatus.Spawned);
      _spawnedPlayer = playerOp.Object;
      _spawnedPlayer.name = $"Player: {_spawnedPlayer.Id}";
   } 
   
   public async UniTask ShutdownRunner()
   {
      if (runner != null && runner.IsRunning)
      {
         await runner.Shutdown();
         runner = null;
      }
   }

   public async UniTask JoinLobby()
   {
      if (runner == null)
      {
         InstantiateRunner();
      }

      var result = await runner.JoinSessionLobby(SessionLobby.Shared);
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
      runner = Instantiate(networkRunnerPrefab);
      runner.AddCallbacks(new NetworkCallbackHandler());
   }
}
