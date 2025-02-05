using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviourSingleton<SceneLoadManager>
{
   public static bool isLoaded;
   public string emptySceneName = "Empty";

   private Fader _fader;

   private string Current { get; set; } = string.Empty;

   protected override void Awake()
   {
      var activeScene = SceneManager.GetActiveScene();
      Current = activeScene.name;

      _fader = GetComponent<Fader>();
      
      DontDestroyOnLoad(gameObject);
   }

   public async UniTask LoadScene(string sceneName)
   {
      Debug.Log("SeneLoadManager : LoadScene()");
      await LoadProcess(sceneName);
   }

   private async UniTask LoadProcess(string sceneName)
   {
      /*if (Current == sceneName)
         return;*/
      Debug.Log($"LoadScene {sceneName}");

      await _fader.Show();

      await SceneManager.LoadSceneAsync(emptySceneName, LoadSceneMode.Additive);

      if (!string.IsNullOrEmpty(Current))
         await SceneManager.UnloadSceneAsync(Current);

      await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
      Current = sceneName;
      
      isLoaded = false;
      await UniTask.WaitUntil(() => isLoaded);
      
      await SceneManager.UnloadSceneAsync(emptySceneName);
      await _fader.Hide();
   }
}
