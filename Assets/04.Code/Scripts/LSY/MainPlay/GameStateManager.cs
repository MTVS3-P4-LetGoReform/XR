using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public WebApiData webApiData;
    private ModelgenerateController _modelgenerateController;
    private static GameStateManager _instance;
    public GameObject completeScreen;
    public GameObject otherScreen;
    public GameObject guideObjects;
    public GameObject pedestral;
    
    public bool isComplete = false;

    public int maxCnt = -1;
    public int allCnt = 0;
    
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameStateManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(nameof(GameStateManager));
                    _instance = singletonObject.AddComponent<GameStateManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        _modelgenerateController = FindObjectOfType<ModelgenerateController>();
        _modelgenerateController.GeneratePlayModel();
        StartCoroutine(CompleteCoroutine());
        // FIXME : 복셀화 끝나고 나서 멀티쪽 동기화 호출
    }

    

    private IEnumerator CompleteCoroutine()
    {
        // 소리 및 효과 재생
        yield return new WaitUntil(() => isComplete);
        guideObjects.SetActive(false);
        otherScreen.SetActive(false);
        pedestral.SetActive(false);
        yield return new WaitForSeconds(5f);
        Cursor.lockState = CursorLockMode.None;
        completeScreen.SetActive(true);
    }

    public void DoCompleteCoroutine()
    {
        isComplete = true;
        //StartCoroutine(CompleteCoroutine());
    }

    public void SetMaxVoxelCnt(int num)
    {
        maxCnt = num;
        allCnt = 0;
    }

    public void AddCnt(int num)
    {
        allCnt += num; 
    }

    public bool IsComplete()
    {
        return allCnt == maxCnt;
    }
}
