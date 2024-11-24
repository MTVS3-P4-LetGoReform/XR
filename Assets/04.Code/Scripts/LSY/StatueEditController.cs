using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using GLTFast;
using UnityEngine.UI;

public class StatueEditController : MonoBehaviour
{
    private int selectedObjectIndex = -1;
    public event Action OnClicked, OnExit;
    private Coroutine currentCoroutine;
    private bool isInstantitate = false;
    private StatueInventoryController statueInventoryController;
    private StatueInventoryUIController statueInventoryUIController;
    private RaycastHit Hit;
    private Vector3 pos;
    
    public Camera playerCamera;
    private GameObject newPreviewPrefab;
    [SerializeField] private LayerMask BFLayerMask; 
    
    void Start()
    {
        StopPlacement();
        StartCoroutine(FindPlayerCamera());
        StartCoroutine(FindInventoryController());
        StartCoroutine(FindInventoryUIController());

        for (int i = 0; i < statueInventoryController.inventoryTargetList.Count; i++)
        {
            int curIndex = i;
            
            Debug.Log(i);
            statueInventoryController.inventoryTargetList[i].GetComponent<Button>().onClick.AddListener(() => SetSelectedObjectIndex(curIndex));
        }
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

    IEnumerator FindInventoryController()
    {
        while (statueInventoryController == null)
        {
            statueInventoryController = FindObjectOfType<StatueInventoryController>();
        }
        yield return null;
    }
    
    IEnumerator FindInventoryUIController()
    {
        while (statueInventoryUIController == null)
        {
            statueInventoryUIController = FindObjectOfType<StatueInventoryUIController>();
        }
        yield return null;
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
        
        // 설치할 오브젝트 생성
        StatueData statueData = statueInventoryController.statueDatas[selectedObjectIndex];
        GltfImport gltfImport = statueData.modelGltf;
        GameObject glbObject = new GameObject("GLBModel");
        
        // 설치할 오브젝트 위치 설정
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
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
            LandManager.PlacedObjects[landObject.key] = glbObject;
            RealtimeDatabase.AddObjectToUserLand(UserData.Instance.UserId,landObject); //실제코드 
        }
    }

    public void SetSelectedObjectIndex(int idx)
    {
        selectedObjectIndex = idx;
        statueInventoryUIController.ActivateInstallBtn(idx);
        Debug.Log("selectedObjectIndex : "+idx);
    }
    
}