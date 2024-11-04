using System.Collections;
using TMPro;
using UnityEngine;

public class TestInteriorMode : MonoBehaviour
{
    public GameObject craftHammer;
    public GameObject testWall;
    public GameObject testShadow;
    private GameObject newTestShadow;
    public TMP_Text stateText;
    public GameObject boxLine;
    public Canvas interiorCanvas;
    private RaycastHit Hit;
    private bool isHammer;
    [SerializeField] private LayerMask BFLayerMask;
    [SerializeField] public LayerMask PBLayerMask;

    void Start()
    {
        stateText.text = "InviteMode";
        newTestShadow = Instantiate(testShadow, new Vector3(0, -20, 0), Quaternion.identity);

    }

    void Update()
    {
        InteriorModeTrigger();
        boxLine.gameObject.SetActive(false);
    }

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

            pos += Vector3.one * 0.5f;

            newTestShadow.transform.position = pos;
            
    
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(testWall, pos, Quaternion.identity);
            }
        }
    }

   
}
