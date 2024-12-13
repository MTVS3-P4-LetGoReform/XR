using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using GLTFast;
using UnityEngine.UI;

public class StatueEditController : MonoBehaviour
{
    [SerializeField] private LandObjectController objectController;

    private int selectedObjectIndex = -1;
    public event Action OnClicked, OnExit;
    private Coroutine currentCoroutine;
    private bool isInstantitate = false;
    private StatueInventoryController statueInventoryController;
    private StatueInventoryUIController statueInventoryUIController;
    private RaycastHit Hit;
    private Vector3 pos;
    public float scaleFactor = 3f;

    public Material previewMat;
    
    [SerializeField] private LayerMask BFLayerMask; 
    [SerializeField] private LayerMask IPLayerMask;
    [SerializeField] private TestInteriorMode _testInteriorMode;
    [SerializeField] private GameObject newPreviewPrefab;
    
    void Start()
    {
        StopPlacement();
        _testInteriorMode = FindObjectOfType<TestInteriorMode>();
        StartCoroutine(FindInventoryController());
        StartCoroutine(FindInventoryUIController());

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            OnExit?.Invoke();
            Cursor.lockState = CursorLockMode.None;
            statueInventoryUIController.SetInventoryMode();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            statueInventoryUIController.SetInventoryMode();
        }
        
    }
    
    public void StartPlacement(int idx) // 클릭하는 버튼에 할당
    {
        Debug.Log("StatueEditController : StartPlacement()");
        if (idx < 0)
        {
            Debug.LogError($"No index Found{idx}");
            return;
        }
        selectedObjectIndex = idx;
        Debug.Log($"selectedObjectIndex : {selectedObjectIndex}");

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            //해당 코루틴을 1번만 실행시켜주게 하는 코드
        }

        //currentCoroutin = StartCoroutine(PreviewObjectMoveController());

        currentCoroutine = StartCoroutine(PreviewObjectMoveController());

        OnClicked += PlaceStructure; // PlaceStructure 메소드 구독
        OnExit += StopPlacement; // StopPlacement 메소드 구독

        Cursor.lockState = CursorLockMode.Locked;

    }
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
    

    IEnumerator FindInventoryController()
    {
        while (statueInventoryController == null)
        {
            Debug.Log("FindInventoryController");
            statueInventoryController = FindObjectOfType<StatueInventoryController>();
            yield return null;
        }

        Debug.Log($"statueInventoryController - {statueInventoryController}");
        InitializeInventoryTargets();
    }
    
    void InitializeInventoryTargets()
    {
        for (int i = 0; i < statueInventoryController.inventoryTargetList.Count; i++)
        {
            int curIndex = i;
            statueInventoryController.installBtns[i].onClick.AddListener(() => StartPlacement(curIndex));
            statueInventoryController.inventoryTargetList[i].GetComponent<Button>().onClick
                .AddListener(() => ActivateInstallBtn(curIndex));
        }
    }
    
    IEnumerator FindInventoryUIController()
    {
        while (statueInventoryUIController == null)
        {
            statueInventoryUIController = FindObjectOfType<StatueInventoryUIController>();
            yield return null;
        }
        
    }
    
    public bool IsPointerOnUI()
        => EventSystem.current.IsPointerOverGameObject(); // EventSystem에서 현재 마우스위치가 UI위에 있는지 판별
    
     private async void PlaceStructure() // 카메라Ray의 위치에 선택한 가구 배치
    {
        Debug.Log($"StatueEditController : PlaceStructure");
        if (IsPointerOnUI()) // 판별값이 true일때 placeStructure 실행
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        // 설치할 오브젝트 생성
        StatueData statueData = statueInventoryController.statueDatas[selectedObjectIndex];
        Debug.Log($"statueData Id : {statueData.modelId}");
        GltfImport gltfImport = statueData.modelGltf;
        Debug.Log($"statueData gltfImport : {statueData.modelGltf}");
        GameObject glbObject = new GameObject("GLBModel");
        
        // 설치할 오브젝트 위치 설정
        Ray ray = new Ray(_testInteriorMode.userCamera.transform.position, _testInteriorMode.userCamera.transform.forward);
        
        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
        {
            // 설치할 레이캐스트 위치 가져옴
            pos = Hit.point;

            // 설치할 위치 변환
            pos = new Vector3(
                Mathf.Floor(pos.x),
                Mathf.Floor(pos.y),
                Mathf.Floor(pos.z));

            Debug.Log($"StatueInventoryDB : pos - {pos}");
            
            // glbObject 하위에 gltf 오브젝트 설치
            await gltfImport.InstantiateMainSceneAsync(glbObject.transform);
            glbObject.transform.position = pos;
            glbObject.transform.localScale = new Vector3(3f, 3f, 3f);

            // 설치한 모델 정보 DB에 저장
            var landObject = LandObjectConverter.ConvertToModelObject(glbObject);
            LandObjectController.AddPlacedObject(landObject.key,glbObject);
            RealtimeDatabase.AddObjectToUserLandAsync(UserData.Instance.UserId,landObject).Forget(); //실제코드 
        }
    }

    public void ActivateInstallBtn(int idx)
    {
        Debug.Log($"ActivateInstallBtn {idx}");
        statueInventoryController.installBtns[idx].gameObject.SetActive(true);
    }

    private IEnumerator PreviewObjectMoveController()
    {
        
        Debug.Log("PreviewObjectMoveController");
        if (!isInstantitate)
        {
            Debug.Log("isInsantiate = flase");
            yield return CreatePreviewObjectAsync(); // 비동기 메서드를 호출
            isInstantitate = true; // 생성 완료 플래그 설정
        }
        
        while (true)
        {
            Debug.Log("while");
            Ray ray = new Ray(_testInteriorMode.userCamera.transform.position,
                _testInteriorMode.userCamera.transform.forward);
            if (Physics.Raycast(ray, out Hit, Mathf.Infinity, IPLayerMask))
            {
                pos = Hit.point;

                pos = new Vector3(
                    Mathf.Floor(pos.x),
                    Mathf.Floor(pos.y),
                    Mathf.Floor(pos.z));
                Debug.Log($"pos : {pos}");
                newPreviewPrefab.transform.position = pos;
            }
            
            yield return null;
        }
    }
    private async Task CreatePreviewObjectAsync()
    {
        Debug.Log("CreatePreviewObjectAsync start");
        GltfImport gltfImport = statueInventoryController.statueDatas[selectedObjectIndex].modelGltf;
        newPreviewPrefab = new GameObject("PreviewModel");
        Vector3 scaleVec3 = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        newPreviewPrefab.transform.localScale = scaleVec3;
        await gltfImport.InstantiateMainSceneAsync(newPreviewPrefab.transform); // 비동기 작업
        newPreviewPrefab.GetComponentInChildren<Renderer>().material = previewMat;
        Debug.Log("CreatePreviewObjectAsync Finish");
    }
}