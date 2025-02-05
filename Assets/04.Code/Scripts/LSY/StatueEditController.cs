﻿using Cysharp.Threading.Tasks;

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
    private InpaingtingWebCommManager inpaingtingWebCommManager;
    private RaycastHit Hit;
    private Vector3 pos;
    public float scaleFactor = 3f;

    public Material previewMat;

    private Quaternion newPreviewStatueRotate;
    private Quaternion originRotation;
    
    [SerializeField] private LayerMask BFLayerMask; 
    [SerializeField] private LayerMask IPLayerMask;
    [SerializeField] private TestInteriorMode _testInteriorMode;
    [SerializeField] private GameObject newPreviewPrefab;

    private StatueData targetStatueData;

    public GameObject ToypicAreaPrefab;

    public WebApiData webApiData;
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
            OnExit?.Invoke();
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
        targetStatueData = statueInventoryController.statueDatas[selectedObjectIndex];
        Debug.Log($"statueData Id : {targetStatueData.modelId}");
        GltfImport gltfImport = targetStatueData.modelGltf;
        Debug.Log($"statueData gltfImport : {targetStatueData.modelGltf}");
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
            
            // Toypick Area 설치
            GameObject toypicArea = Instantiate(ToypicAreaPrefab, pos, newPreviewStatueRotate);
            // glbObject 하위에 gltf 오브젝트 설치
            await gltfImport.InstantiateMainSceneAsync(glbObject.transform);
            //glbObject.transform.position = pos;
            
            glbObject.transform.rotation = newPreviewStatueRotate;
            
            // glbObject의 부모를 toypicArea로 설정
            glbObject.transform.SetParent(toypicArea.transform);
            // foreach (Transform child in glbObject.transform)
            // {
            //     child.SetParent(toypicArea.transform, worldPositionStays: false);
            // }
            
            glbObject.transform.localScale = new Vector3(3f, 3f, 3f);
            glbObject.transform.localPosition = Vector3.zero;
            
            // 설치된 statueData 설정
            // FIXME : 왜 안되지
            toypicArea.GetComponentInChildren<InpaingtingWebCommManager>().statueData.imageName = webApiData.ImageName;
            toypicArea.GetComponentInChildren<InpaingtingWebCommManager>().statueData.creatorId = webApiData.UserId;
            // Debug.Log($"StatueEditController : {toypicArea.GetComponentInChildren<InpaingtingWebCommManager>()}");
            // toypicArea.GetComponentInChildren<InpaingtingWebCommManager>().statueData = targetStatueData;
            // Debug.Log($"StatueEditController : {toypicArea.GetComponentInChildren<InpaingtingWebCommManager>().statueData.imageName}");
            // 설치한 모델 정보 DB에 저장 
            var landObject = LandObjectConverter.ConvertToModelObject(glbObject);
            LandObjectController.AddPlacedObject(landObject.key,glbObject);
            RealtimeDatabase.AddObjectToUserLandAsync(UserData.Instance.UserId,landObject).Forget(); //실제코드 
            
            newPreviewStatueRotate = originRotation;
            newPreviewPrefab.transform.rotation = originRotation;
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
                
                if (!isInstantitate)
                {
                    Debug.Log("isInsantiate = flase");
                    yield return CreatePreviewObjectAsync(); // 비동기 메서드를 호출
                    isInstantitate = true; // 생성 완료 플래그 설정
                }
                
                if (newPreviewPrefab != null)
                {
                    newPreviewPrefab.transform.position = pos;
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        newPreviewPrefab.transform.Rotate(Vector3.down, 90f, Space.World);
                        newPreviewStatueRotate = newPreviewPrefab.transform.rotation;
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        newPreviewPrefab.transform.Rotate(Vector3.up, 90f, Space.World);
                        newPreviewStatueRotate = newPreviewPrefab.transform.rotation;
                    }
                }
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
        originRotation = Quaternion.identity;
        Debug.Log("CreatePreviewObjectAsync Finish");
    }
}