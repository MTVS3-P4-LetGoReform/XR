using System;
using System.Collections;
using System.IO;
using GLTFast;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public Action<bool> Complete;
    public WebApiData webApiData;
    private ModelgenerateController _modelgenerateController;
    private static GameStateManager _instance;
    public GameObject completeScreen;
    public GameObject otherScreen;
    public GameObject guideObjects;
    public GameObject pedestral;
    public GameObject modelAreaObject;
    public bool isComplete = false;
    public int maxCnt = int.MaxValue;
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
        LoadAndInstantiateGLB(webApiData.ModelName);
        _modelgenerateController = FindObjectOfType<ModelgenerateController>();
        
    }
    
    private void Start()
    {
        
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
        Complete?.Invoke(true);
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
    public async void LoadAndInstantiateGLB(string fName)
    {
        //string folderPath = Path.Combine(Application.persistentDataPath, "Models");
        string filePath = Path.Combine(Application.persistentDataPath,"Models",fName);
        //string filePath = Path.Combine(Application.dataPath, "DownloadedGLB", "958462d6cbd04f441aad81e88529b8a2.glb");
        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // glTFast를 이용해 GLB 파일을 로드
            GltfImport gltfImport = new GltfImport();
            var success = await gltfImport.Load(filePath); // 비동기 로드 처리
            // 파일 로드에 성공하면 씬에 배치
            if (success)
            {
                
                //GameObject glbObject = new GameObject("GLBModel");
                //glbObject.tag = "GLBModel";
                //glbObject.transform.SetParent(modelAreaObject.transform);
                //glbObject.transform.localPosition = Vector3.zero;
                //await
                webApiData.ModelGltfImport = gltfImport;
                await gltfImport.InstantiateMainSceneAsync(modelAreaObject.transform);
                Debug.Log("GLB file instantiated in scene.");
                GameObject generatedObject = modelAreaObject.transform.GetChild(modelAreaObject.transform.childCount - 1).gameObject;
                _modelgenerateController.GeneratePlayModel(generatedObject);
                StartCoroutine(CompleteCoroutine());
            }
            else
            {
                Debug.LogError("Failed to load GLB file using glTFast.");
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }
    }

    public void StartGameProcess()
    {
        _modelgenerateController.AdvanceLayer(); 
    }
}
