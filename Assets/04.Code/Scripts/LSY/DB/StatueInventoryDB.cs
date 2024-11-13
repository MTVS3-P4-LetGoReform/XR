using System;
using System.Collections;
using System.IO;
using GLTFast;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatueInventoryDB: MonoBehaviour
{
    private string _userId;
    private string[] hasModel;
    private string[] hasModelName;
    private StorageDatabase _storageDatabase;
    public WebApiData webApiData;
    private RaycastHit Hit;
    private Vector3 pos;
    [SerializeField] private LayerMask BFLayerMask; 
    public event Action OnClicked, OnExit; // Action 델리게이트를 사용하여 메소드 선언
    private int selectedObjectIndex = -1;
    private Coroutine currentCoroutine;
    private GameObject newPreviewPrefab;
    private bool isInstantitate = false;
    private Quaternion originRotation;
    private Quaternion newPreviewPrefabRatate;
    public Camera playerCamera;

    public GameObject statueInventoryCanvasObject;
    void Start()
    {
        StopPlacement();
        StartCoroutine(FindPlayerCamera());
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke(); // 마우스 좌클릭시, OnClicked에 구독된 모든 메서드 호출
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
            Cursor.lockState = CursorLockMode.None;
        }

        if (selectedObjectIndex < 0)
        {
            return;
        }
    }
    
    public void StartPlacement() // 클릭하는 버튼에 할당
    {
        // StopPlacement();

        // if (currentCoroutine != null)
        // {
        //     StopCoroutine(currentCoroutine);
        //     currentCoroutine = null;
        //     //해당 코루틴을 1번만 실행시켜주게 하는 코드
        // }
        
        //currentCoroutine = StartCoroutine(PreviewObjectMoveController());
        

        OnClicked += PlaceStructure; // PlaceStructure 메소드 구독
        OnExit += StopPlacement; // StopPlacement 메소드 구독

        Cursor.lockState = CursorLockMode.Locked;

    }
    
    // IEnumerator PreviewObjectMoveController() // PreviewPrefab을 이동시키기 위한 코드
    // {
    //     string filePath = Path.Combine(Application.persistentDataPath, webApiData.ModelName);
    //     GltfImport gltfImport = new GltfImport();
    //     bool success = await gltfImport.Load(filePath); // 비동기 로드 처리
    //     
    //         
    //     // 파일 로드에 성공하면 씬에 배치
    //     if (success)
    //     { 
    //         while (true)
    //         {
    //             GameObject glbObject = new GameObject("GLBModel");
    //             Ray ray =  new Ray(Camera.main.transform.position, Camera.main.transform.forward);
    //             if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
    //             {
    //                 pos = Hit.point;
    //                 pos = new Vector3(
    //                     Mathf.Floor(pos.x),
    //                     Mathf.Floor(pos.y),
    //                     Mathf.Floor(pos.z));
    //
    //                pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);
    //
    //             
    //                 if (!isInstantitate)
    //                 {
    //                     await gltfImport.InstantiateMainSceneAsync(glbObject.transform);
    //                     
    //                     isInstantitate = true;
    //
    //                     originRotation = Quaternion.identity;
    //                     // bool type의 isInstantiate가 1번만 실행
    //                 }
    //             
    //                 if (newPreviewPrefab != null)
    //                 {
    //                     newPreviewPrefab.transform.position = pos;
    //                     if (Input.GetKeyDown(KeyCode.Q))
    //                     { 
    //                         newPreviewPrefab.transform.Rotate(Vector3.down, 90f, Space.World);
    //                         newPreviewPrefabRatate = newPreviewPrefab.transform.rotation;
    //                     }
    //
    //                     if (Input.GetKeyDown(KeyCode.E))
    //                     {
    //                         newPreviewPrefab.transform.Rotate(Vector3.up, 90f, Space.World);
    //                         newPreviewPrefabRatate = newPreviewPrefab.transform.rotation;
    //                     }
    //                 }
    //             }
    //             yield return null;
    //         }
    //     }
    // }
    
    public void StopPlacement()
    {
        
        selectedObjectIndex = -1;
        OnClicked -= PlaceStructure;
        OnExit -= StopPlacement;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        
        Destroy(newPreviewPrefab);
        isInstantitate = false;
    }
    
    public bool IsPointerOnUI()
        => EventSystem.current.IsPointerOverGameObject(); // EventSystem에서 현재 마우스위치가 UI위에 있는지 판별
    
    private async void PlaceStructure() // 카메라Ray의 위치에 선택한 가구 배치
    {
        if (IsPointerOnUI()) // 판별값이 true일때 placeStructure 실행
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        
        
            Debug.Log("StatueInventoryDB : LoadAndInstantiateGLB()");
        string filePath = Path.Combine(Application.persistentDataPath, webApiData.ModelName);
        //string filePath = Path.Combine(Application.dataPath, "DownloadedGLB", "958462d6cbd04f441aad81e88529b8a2.glb");
        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // glTFast를 이용해 GLB 파일을 로드
            GltfImport gltfImport = new GltfImport();
            bool success = await gltfImport.Load(filePath); // 비동기 로드 처리

            // 파일 로드에 성공하면 씬에 배치
            if (success)
            {
                GameObject glbObject = new GameObject("GLBModel");
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
                {
                    pos = Hit.point;

                    pos = new Vector3(
                        Mathf.Floor(pos.x),
                        Mathf.Floor(pos.y),
                        Mathf.Floor(pos.z));

                    //pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);
                    Debug.Log($"StatueInventoryDB : pos - {pos}");
                    await gltfImport.InstantiateMainSceneAsync(glbObject.transform);
                    glbObject.transform.position = pos;
                    //Debug.Log("GLB file instantiated in scene.");
                    var landObject = LandObjectConverter.ConvertToModelObject(glbObject);
                    LandManager.PlacedObjects[landObject.key] = glbObject;
                    //RealtimeDatabase.AddObjectToUserLand("TestUser",landObject); // 테스트코드
                    RealtimeDatabase.AddObjectToUserLand(UserData.Instance.UserId,landObject); //실제코드 
            
                    //newPreviewPrefabRatate = originRotation;
                    //newPreviewPrefab.transform.rotation = originRotation;
                }
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
    
    
    
    
    //private void Start()
    //{
        // _storageDatabase = new StorageDatabase(webApiData);
        // if (UserData.Instance ==null)
        //     return;
        // _userId = UserData.Instance.UserId;
        //
        // LoadUserModelData();
        
    //}

    // public void LoadUserModelData()
    // {
    //     RealtimeDatabase.GetModelList(_userId, modelList =>
    //         {
    //             if (modelList != null)
    //             {
    //                 Debug.Log($"StatueInventoryDB : modelList - {modelList}");
    //                 foreach (var model in modelList.models)
    //                 {
    //                     Debug.Log($"StatueInventoryDB : model - {model}");
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogError("StaueInventoryDB : 모델리스트 데이터를 찾을 수 없음.");
    //             }
    //         },
    //         error => { Debug.LogError($"StatueInventoryDB : 데이터 읽는 중 오류 -{error.Message}"); }
    //     );
    // }

    IEnumerator FindPlayerCamera()
    {
        while (playerCamera == null)
        {
            GameObject playerCamObj = GameObject.FindWithTag("PlayerCamera");

            if (playerCamObj != null)
            {
                playerCamera = playerCamObj.GetComponent<Camera>();
            }

            yield return null;
        }
    }

    public void OpenInventory()
    {
        statueInventoryCanvasObject.SetActive(true);
    }
    
}