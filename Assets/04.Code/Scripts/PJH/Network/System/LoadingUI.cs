using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviourSingleton<LoadingUI>
{
    public GameObject loadingScreen;
    public Slider progressBar;
    
    public async UniTask LoadScene(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        progressBar.gameObject.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        if(operation != null)
            operation.allowSceneActivation = false; // 씬 자동 전환 방지

        float targetProgress = 0;

        while (operation != null && !operation.isDone)
        {
            // 실제 진행률 가져오기
            float actualProgress = Mathf.Clamp01(operation.progress / 0.9f);
        
            // targetProgress를 실제 진행률까지 서서히 증가시키기
            targetProgress = Mathf.MoveTowards(targetProgress, actualProgress, 0.02f); // 게이지 상승 속도 조절
            progressBar.value = targetProgress;

            // 지연 추가
            await UniTask.Delay(100);
        }

        // 로딩 완료 후, 게이지가 끝까지 채워지도록 보장
        progressBar.value = 1f;

        if (operation != null)
            operation.allowSceneActivation = true; // 씬 활성화
       

        progressBar.gameObject.SetActive(false);
        loadingScreen.SetActive(false);
    }
}
