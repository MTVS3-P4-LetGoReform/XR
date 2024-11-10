using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Cysharp.Threading.Tasks;

public class LoadingSceneController : MonoBehaviour
{
    #region Singleton

    private static LoadingSceneController _instance;
    public static LoadingSceneController Instance
    {
        get
        {
            if (_instance == null)
            {
                LoadingSceneController sceneController = FindAnyObjectByType<LoadingSceneController>();
                if (sceneController != null)
                {
                    _instance = sceneController;
                }
                else
                {
                    // 인스턴스가 없다면 생성
                    _instance = Create();
                }
            }

            return _instance;
        }
    }

    #endregion

    private static LoadingSceneController Create()
    {
        // 리소스에서 로드
        return Instantiate(Resources.Load<LoadingSceneController>("LoadingUI"));
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private CanvasGroup mCanvasGroup;
    [SerializeField] private Slider mProgressBar;
   
    private int _sceneNum;

    private Func<UniTask> _mOnSceneLoadAction;

    public async UniTask LoadScene(int num, Func<UniTask> action = null)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        _mOnSceneLoadAction = action;
        
        _sceneNum = num;
        await LoadSceneProcess();
    }

    private async UniTask LoadSceneProcess()
    {
        mProgressBar.value = 0.0f;

        //코루틴 안에서 yield return으로 코루틴을 실행하면.. 해당 코루틴이 끝날때까지 대기한다
        await Fade(true);

        //로컬 로딩
        AsyncOperation op = SceneManager.LoadSceneAsync(_sceneNum);
        
        if(op != null)
            op.allowSceneActivation = false;

        float process = 0.0f;

        //씬 로드가 끝나지 않은 상태라면?
        while (op != null && !op.isDone)
        {
            await UniTask.Yield();

            if (op.progress < 0.9f)
            {
                mProgressBar.value = op.progress;
            }
            else
            {
                process += Time.deltaTime * 5.0f;
                mProgressBar.value = Mathf.Lerp(0.9f, 1.0f, process);

                if (process > 1.0f)
                {
                    op.allowSceneActivation = true;
                    return;
                }
            }
        }
    }
    
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == _sceneNum)
        {
            Fade(false).Forget();
            SceneManager.sceneLoaded -= OnSceneLoaded;

            // 로딩이 완료된 후 씬에서 PlayerSpawn을 호출
            _mOnSceneLoadAction?.Invoke().Forget();
        }
    }


    private IEnumerator CoLateStart()
    {
        yield return new WaitForEndOfFrame();

        // 예약된 함수 실행
        _mOnSceneLoadAction?.Invoke();
    }

    private async UniTask Fade(bool isFadeIn)
    {
        float process = 0f;

        if (!isFadeIn)
            StartCoroutine(CoLateStart());

        while (process < 1.0f)
        {
            process += Time.unscaledDeltaTime;
            mCanvasGroup.alpha = isFadeIn ? Mathf.Lerp(0.0f, 1.0f, process) : Mathf.Lerp(1.0f, 0.0f, process);

            await UniTask.Yield();
        }

        if (!isFadeIn)
            gameObject.SetActive(false);
    }
}