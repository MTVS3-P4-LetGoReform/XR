using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TestBlockCreateRayCastController : MonoBehaviour
{
    public GameObject BlockPrefab;
    public GameObject BlockOutline;
    public TMP_Text blockCountText;
    public TMP_Text noBlockText;
    public LayerMask BFLayerMask;
    private RaycastHit Hit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        BFLayerMask = LayerMask.GetMask("Block", "Floor");
    }

    // Update is called once per frame
    void Update()
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

            BlockOutline.transform.position = pos;

            if (Input.GetKey(KeyCode.F))
            {
                BlockOutline.gameObject.SetActive(false);
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                BlockOutline.gameObject.SetActive(true);
            }

            
            if (Input.GetMouseButtonDown(0))
            {
                if ( SharedBlockData.BlockNumber > 0)
                {
                    Instantiate(BlockPrefab, pos, Quaternion.identity);

                    SharedBlockData.BlockNumber -= 1;
                    blockCountText.text = $"{SharedBlockData.BlockNumber}";
                }
                else
                {
                    Debug.Log("No Block!!");
                    StartCoroutine(NoBlockTextSet());
                }
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                if (Hit.collider.name == "BasicBlock(Clone)" || Hit.collider.name == "PhysicsBasicBlock(Clone)")
                {
                    Destroy(Hit.collider.gameObject);


                    SharedBlockData.BlockNumber += 1;
                    blockCountText.text = $"{SharedBlockData.BlockNumber}";
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 99999);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Hit.point, 0.05f);
        
        Gizmos.DrawRay(Hit.point, Hit.normal);
    }

    IEnumerator NoBlockTextSet()
    {
        noBlockText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        noBlockText.gameObject.SetActive(false);
    }
    
}
