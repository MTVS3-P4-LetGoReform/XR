using Cysharp.Threading.Tasks;
using Fusion;
using Photon.Voice.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RunnerManager : MonoBehaviour
{
   public static RunnerManager Instance { get; private set; }
   
   public NetworkPrefabRef playerPrefab; 
   public NetworkPrefabRef sharedGameDataPrefab;
   public NetworkRunner networkRunnerPrefab;
   
   public NetworkRunner runner;
   
   private NetworkObject _spawnedPlayer;
   private NetworkObject _sharedGameData;
   private Transform _currentSpawnPoint;
   
   #region SpawnPoint
   [SerializeField] private Transform publicParkSpawnPoint;
   [SerializeField] private Transform playSpawnPoint;
   [SerializeField] private Transform personalSpawnPoint;
   #endregion
   
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
      await FirstStartGame(); 
   }

   public async UniTask RunnerStart(StartGameArgs args,int sceneIndex)
   {
      if (runner == null)
      {
         InstantiateRunner();
      }

      var result = await runner.StartGame(args);
      if (result.Ok)
      {
         Debug.Log($"세션이름: '{args.SessionName}'이 만들어졌습니다.");

         LoadScene(sceneIndex);
         await UniTask.WaitUntil(() => SceneLoadManager.isLoaded);
         
         await PlayerSpawn();
         Debug.Log("캐릭터 생성");
         
         if (SceneManager.GetActiveScene().name == "3.Test_PlayScene")
         {
            await SharedGameDataSpawn();
            Debug.Log("공유게임정보 생성");
         }
      }
      else
      {
         Debug.LogError($"세션 생성에 실패했습니다.: {result.ShutdownReason}");
      }
   }

   private void LoadScene(int sceneIndex)
   {
      Debug.Log("Runner Manager : LoadScene - "+ sceneIndex);
      var sceneName = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
      SceneLoadManager.Instance.LoadScene(sceneName);
      Debug.Log("씬이름 : " + sceneIndex);
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
      var startArgs = new StartGameArgs
      {
         GameMode = GameMode.Shared,
         SessionName = "공용 세션"
      };
      
      await ShutdownRunner();
      await RunnerStart(startArgs,1);
   }
   
   private async UniTask FirstStartGame()
   {
      InstantiateRunner();
      
      var startArgs = new StartGameArgs
      {
         GameMode = GameMode.Shared,
         SessionName = "공용 세션"
      };

      var result = await runner.StartGame(startArgs);
      if (result.Ok)
      {
         await PlayerSpawn();
         Debug.Log($"세션이름: '{startArgs.SessionName}'이 만들어졌습니다.");
      }
      else
      {
         Debug.LogError($"세션 생성에 실패했습니다.: {result.ShutdownReason}");
      }
   }

   private async UniTask PlayerSpawn()
   {
      switch (SceneManager.GetActiveScene().buildIndex)
      {
         case 1:
            _currentSpawnPoint = publicParkSpawnPoint;
            break;
         case 2:
            _currentSpawnPoint = playSpawnPoint;
            break;
         case 3:
            _currentSpawnPoint = personalSpawnPoint;
            break;
         default:
            _currentSpawnPoint = publicParkSpawnPoint;
            break;
      }
      
      var playerOp = runner.SpawnAsync(playerPrefab,_currentSpawnPoint.position,quaternion.identity);
      await UniTask.WaitUntil(() => playerOp.Status == NetworkSpawnStatus.Spawned);
      _spawnedPlayer = playerOp.Object;
      _spawnedPlayer.name = $"Player: {_spawnedPlayer.Id}";
   }

   private async UniTask SharedGameDataSpawn()
   {
      var sharedGameDataOp = runner.SpawnAsync(sharedGameDataPrefab);
      await UniTask.WaitUntil(() => sharedGameDataOp.Status == NetworkSpawnStatus.Spawned);
      _sharedGameData = sharedGameDataOp.Object;
      _sharedGameData.name = $"Player: {_sharedGameData.Id}";
   }
   private void InstantiateRunner()
   {
      var recorder = GameObject.FindAnyObjectByType<Recorder>(FindObjectsInactive.Include);
      Debug.Log($"Recorder : {recorder}");
      
      runner = Instantiate(networkRunnerPrefab);
      runner.AddCallbacks(new NetworkCallbackHandler());
   }
}
