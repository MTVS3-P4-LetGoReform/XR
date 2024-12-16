using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestInteriorMode : MonoBehaviour
{
    public Canvas interiorCanvas;
    public AudioSource audioPlaceSound;
    [SerializeField] private GameObject newPreviewPrefab;



    private Quaternion newPreviewPrefabRatate;
    private Quaternion originRotation;
    private RaycastHit Hit;
    private bool isInstantitate = false;

    private Coroutine currentCoroutine;

    private float rotateSpeed = 200f;

    

    [SerializeField] public Camera userCamera;
    [SerializeField] private GameObject newUserCamera;


    private Vector3 pos;
    public event Action OnClicked, OnExit, OnPushed; // Action 델리게이트를 사용하여 메소드 선언

    [SerializeField] private ObjectDatabase objectDatabase; // ScriptableObject 호출
    private int selectedObjectIndex = -1;

    private bool isHammer;
    [SerializeField] private LayerMask IPLayerMask;
    [SerializeField] private LayerMask IILayerMask;
    [SerializeField] private LayerMask STLayerMask;
    

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

        if (Input.GetMouseButtonDown(1))
        {
            DestroyInstallation();
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
        OnExit += StopPlacement; // StopPlacement
        

        Cursor.lockState = CursorLockMode.Locked;

    }

    IEnumerator PreviewObjectMoveController() // PreviewPrefab을 이동시키기 위한 코드
    {
        while (true)
        {
            Ray ray = new Ray(userCamera.transform.position, userCamera.transform.forward);
            if (Physics.Raycast(ray, out Hit, 5f, IPLayerMask))
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

    private async void PlaceStructure()
    {
        if (IsPointerOnUI())
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        audioPlaceSound.Play();
        
        Ray ray = new Ray(userCamera.transform.position, userCamera.transform.forward);
        if (Physics.Raycast(ray, out Hit, 5f, IPLayerMask))
        {
            pos = Hit.point;
            pos = new Vector3(
                Mathf.Floor(pos.x),
                Mathf.Floor(pos.y),
                Mathf.Floor(pos.z));
            pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);

            GameObject instantiateObject = Instantiate(objectDatabase.objectData[selectedObjectIndex].Prefab, 
                pos, newPreviewPrefabRatate);

            var landObject = LandObjectConverter.ConvertToLandObject(instantiateObject, selectedObjectIndex);
            
            var identifier = instantiateObject.GetComponent<ObjectIdentifier>();
            identifier.SetKey(landObject.key);
            LandObjectController.AddPlacedObject(landObject.key, instantiateObject);
            await RealtimeDatabase.AddObjectToUserLandAsync(UserData.Instance.UserId, landObject);

            newPreviewPrefabRatate = originRotation;
            newPreviewPrefab.transform.rotation = originRotation;
        }
    }


    private void DestroyInstallation()
    {
        Ray ray = new Ray(userCamera.transform.position, userCamera.transform.forward);
        if (Physics.Raycast(ray, out Hit, 5f, STLayerMask))
        {
            ObjectIdentifier ObI = Hit.collider.GetComponent<ObjectIdentifier>();
            string keyIndex = ObI.Key;
            Debug.Log($"{keyIndex}");
            LandObjectController.DeleteSelectedObject(keyIndex).Forget();
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
