using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestInteriorMode : MonoBehaviour
{
    public GameObject craftHammer;
    public TMP_Text stateText;
    public GameObject boxLine;
    public Canvas interiorCanvas;
    [SerializeField]
    private GameObject newPreviewPrefab;

    private Quaternion newPreviewPrefabRatate;
    private Quaternion originRotation;
    private RaycastHit Hit;
    private bool isInstantitate = false;
    
    private Coroutine currentCoroutine;

    private float rotateSpeed = 200f;

    

    private Vector3 pos;
    public event Action OnClicked, OnExit; // Action 델리게이트를 사용하여 메소드 선언

    [SerializeField] 
    private ObjectDatabase objectDatabase; // ScriptableObject 호출
    private int selectedObjectIndex = -1;
    
    private bool isHammer;
    [SerializeField] private LayerMask BFLayerMask; 
    [SerializeField] public LayerMask PBLayerMask; //추후 특정 사물위에는 특정 사물이 올라가도록 layer정리 예정

    void Start()
    {
        stateText.text = "InviteMode";
        StopPlacement();
    }
    
    void Update()
    {
        InteriorModeTrigger();
        boxLine.gameObject.SetActive(false);
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke(); // 마우스 좌클릭시, OnClicked에 구독된 모든 메서드 호출
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }

        if (selectedObjectIndex < 0)
        {
            return;
        }
    }

    public void StartPlacement(int ID) // 클릭하는 버튼에 할당
    {
       // StopPlacement();
        selectedObjectIndex = objectDatabase.objectData.FindIndex(data => data.ID == ID); // data의 ID속성이 외부의 ID와 같으면 return true
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID Found{ID}");
            return;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(PreviewObjectMoveController());
            currentCoroutine = null;
            //해당 코루틴을 1번만 실행시켜주게 하는 코드
        }

        currentCoroutine = StartCoroutine(PreviewObjectMoveController());

        OnClicked += PlaceStructure; // PlaceStructure 메소드 구독
        OnExit += StopPlacement; // StopPlacement 메소드 구독
        
    }

    IEnumerator PreviewObjectMoveController() // PreviewPrefab을 이동시키기 위한 코드
    {
        while (true)
        {
            Ray ray =  new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
            {
                pos = Hit.point;

                pos = new Vector3(
                    Mathf.Floor(pos.x),
                    Mathf.Floor(pos.y),
                    Mathf.Floor(pos.z));

                pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);

                
                if (!isInstantitate)
                {
                    newPreviewPrefab = Instantiate(objectDatabase.objectData[selectedObjectIndex].PreviewPrefab,
                        pos, Quaternion.identity);
                    isInstantitate = true;

                    originRotation = newPreviewPrefab.transform.rotation;
                    // bool type의 isInstantiate가 1번만 실행
                }
                
                if (newPreviewPrefab != null)
                {
                    newPreviewPrefab.transform.position = pos;
                    if (Input.GetKeyDown(KeyCode.Q))
                    { 
                        newPreviewPrefab.transform.Rotate(Vector3.down, 90f, Space.World);
                        newPreviewPrefabRatate = newPreviewPrefab.transform.rotation;
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        newPreviewPrefab.transform.Rotate(Vector3.up, 90f, Space.World);
                        newPreviewPrefabRatate = newPreviewPrefab.transform.rotation;
                    }
                }

            }
            yield return null;
        }
    }
    
    private void StopPlacement()
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

    private void PlaceStructure() // 카메라Ray의 위치에 선택한 가구 배치
    {
        if (IsPointerOnUI()) // 판별값이 true일때 placeStructure 실행
        {
            return;
        } 
        
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
        { 
            pos = Hit.point;

            pos = new Vector3(
                Mathf.Floor(pos.x),
                Mathf.Floor(pos.y),
                Mathf.Floor(pos.z));

            pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);
            
            GameObject gameObject = Instantiate(objectDatabase.objectData[selectedObjectIndex].Prefab, pos, newPreviewPrefabRatate);
            
            newPreviewPrefab.transform.rotation = originRotation;
            // 추후 설치 방향 재 정립, 설치되는 위치 값을 pivot 기준으로 분류할것인지, 물건 위주로 분류할것인지 구상(후자일 가능성 높음)

        }
    }
    
    
    private void InteriorModeTrigger() // 손에 망치를 드는 메소드, 추후 기능때 수정 예상
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isHammer = !isHammer;
        }

        if (isHammer)
        {
            craftHammer.gameObject.SetActive(true);
            interiorCanvas.gameObject.SetActive(true);
            stateText.text = "InstallMode";
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            craftHammer.gameObject.SetActive(false);
            stateText.text = "InviteMode";
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            interiorCanvas.gameObject.SetActive(false);
        }
    }
}
