using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestInteriorMode : MonoBehaviour
{
    public GameObject craftHammer;
    private GameObject newTestShadow;
    public TMP_Text stateText;
    public GameObject boxLine;
    public Canvas interiorCanvas;
    private RaycastHit Hit;
    public event Action OnClicked, OnExit;

    [SerializeField] 
    private ObjectDatabase objectDatabase;
    private int selectedObjectIndex = -1;
    
    private bool isHammer;
    [SerializeField] private LayerMask BFLayerMask;
    [SerializeField] public LayerMask PBLayerMask;

    void Start()
    {
        stateText.text = "InviteMode";
        StopPlacement();
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        OnClicked -= PlaceStructure;
        OnExit -= StopPlacement;
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = objectDatabase.objectData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID Found{ID}");
            return;
        }

        OnClicked += PlaceStructure;
        OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (IsPointerOnUI())
        {
            return;
        }
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
        {
            Vector3 pos = Hit.point;

            pos = new Vector3(
                Mathf.Floor(pos.x),
                Mathf.Floor(pos.y),
                Mathf.Floor(pos.z));

            pos += (Vector3.right * 0.5f) + (Vector3.forward * 0.5f);
            
            
            
            
            GameObject gameObject = Instantiate(objectDatabase.objectData[selectedObjectIndex].Prefab, pos, Quaternion.identity);
            //gameObject.transform.position = pos;
        }
        
    }

    void Update()
    {
        InteriorModeTrigger();
        boxLine.gameObject.SetActive(false);
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
            
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

    public bool IsPointerOnUI()
        => EventSystem.current.IsPointerOverGameObject();


    private void InteriorModeTrigger()
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
            InteriorInstall();
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

    private void InteriorInstall()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
        {
            Vector3 pos = Hit.point;

            pos = new Vector3(
                Mathf.Floor(pos.x),
                Mathf.Floor(pos.y),
                Mathf.Floor(pos.z));

            pos += Vector3.one;
            
            
        }
    }

   
}
