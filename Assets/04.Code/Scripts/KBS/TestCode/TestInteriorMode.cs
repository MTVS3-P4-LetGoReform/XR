using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestInteriorMode : MonoBehaviour
{
    public GameObject craftHammer;
    public TMP_Text stateText;

    public Canvas interiorCanvas;
    [SerializeField] private GameObject newPreviewPrefab;



    private Quaternion newPreviewPrefabRatate;
    private Quaternion originRotation;
    private RaycastHit Hit;
    private bool isInstantitate = false;

    private Coroutine currentCoroutine;

    private float rotateSpeed = 200f;

    [SerializeField] private Camera userCamera;
    [SerializeField] private GameObject newUserCamera;


    private Vector3 pos;
    public event Action OnClicked, OnExit; // Action 델리게이트를 사용하여 메소드 선언

    [SerializeField] private ObjectDatabase objectDatabase; // ScriptableObject 호출
    private int selectedObjectIndex = -1;

    private bool isHammer;
    [SerializeField] private LayerMask IPLayerMask;
    

    void Start()
    {
        StartCoroutine(FindUserCamera());
        StopPlacement();
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

    public void StartPlacement(int ID) // 클릭하는 버튼에 할당
    {
        // StopPlacement();
        selectedObjectIndex =
            objectDatabase.objectData.FindIndex(data => data.ID == ID); // data의 ID속성이 외부의 ID와 같으면 return true
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID Found{ID}");
            return;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            //해당 코루틴을 1번만 실행시켜주게 하는 코드
        }

        currentCoroutine = StartCoroutine(PreviewObjectMoveController());


        OnClicked += PlaceStructure; // PlaceStructure 메소드 구독
        OnExit += StopPlacement; // StopPlacement 메소드 구독

        Cursor.lockState = CursorLockMode.Locked;

    }

    IEnumerator PreviewObjectMoveController() // PreviewPrefab을 이동시키기 위한 코드
    {
        while (true)
        {
            Ray ray = new Ray(userCamera.transform.position, userCamera.transform.forward);
            if (Physics.Raycast(ray, out Hit, Mathf.Infinity, IPLayerMask))
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

                    originRotation = Quaternion.identity;
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
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }



        Ray ray = new Ray(userCamera.transform.position, userCamera.transform.forward);
        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, IPLayerMask))
        {
            pos = Hit.point;

            pos = new Vector3(
                Mathf.Floor(pos.x),
                Mathf.Floor(pos.y),
                Mathf.Floor(pos.z));

            pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);

            GameObject instantiateObject = Instantiate(objectDatabase.objectData[selectedObjectIndex].Prefab, pos,
                newPreviewPrefabRatate);

            var landObject = LandObjectConverter.ConvertToLandObject(instantiateObject, selectedObjectIndex);
            LandManager.PlacedObjects[landObject.key] = instantiateObject;
            //RealtimeDatabase.AddObjectToUserLand("TestUser",landObject); // 테스트코드
            RealtimeDatabase.AddObjectToUserLand(UserData.Instance.UserId, landObject); //실제코드 

            newPreviewPrefabRatate = originRotation;
            newPreviewPrefab.transform.rotation = originRotation;
            // 추후 설치 방향 재 정립, 설치되는 위치 값을 pivot 기준으로 분류할것인지, 물건 위주로 분류할것인지 구상(후자일 가능성 높음)

        }
    }

    private IEnumerator FindUserCamera()
    {
        newUserCamera = null;

        while (newUserCamera == null)
        {
            newUserCamera = GameObject.FindWithTag("PlayerCamera");

            if (newUserCamera == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                userCamera = newUserCamera.GetComponent<Camera>();
            }
        }
    }
}
