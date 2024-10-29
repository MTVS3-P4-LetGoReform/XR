using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private ModelgenerateController _modelgenerateController;
    private static GameStateManager _instance;
    public GameObject completeScreen;
    public GameObject otherScreen;
    public GameObject guideObjects;
    public GameObject pedestral;
    
    public bool isComplete = false;
    
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
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        _modelgenerateController = FindObjectOfType<ModelgenerateController>();
        _modelgenerateController.GeneratePlayModel();
    }

    public void Update()
    {
        if (isComplete)
        {
            DoCompleteRoutine();
        }
    }

    private void DoCompleteRoutine()
    {
        guideObjects.SetActive(false);
        otherScreen.SetActive(false);
        pedestral.SetActive(false);
        StartCoroutine(CompleteCoroutine());
    }
    
    private IEnumerator CompleteCoroutine()
    {
        // 소리 및 효과 재생
        yield return new WaitForSeconds(5f);
        Cursor.lockState = CursorLockMode.None;
        completeScreen.SetActive(true);
    }
    
}
