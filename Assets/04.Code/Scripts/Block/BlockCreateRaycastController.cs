using System.Collections;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;

public class BlockCreateRaycastController : NetworkBehaviour
{
    public GameObject BlockPrefab;
    public GameObject BlockOutline;
    public GameObject NewBlockOutLine;
    public TMP_Text noBlockText;
    public TMP_Text blockCountText;
    public LayerMask BFLayerMask;
    public LayerMask PBLayerMask;
    public BlockData blockData;
    [SerializeField]
    private Camera camera;
    private RaycastHit Hit;
    private GameObject BasicBlockParent;

    private ModelPlacementChecker _modelPlacementChecker;

    void Start()
    {
        _modelPlacementChecker = FindObjectOfType<ModelPlacementChecker>();
        BasicBlockParent = GameObject.Find("BasicBlockParent");
        camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        NewBlockOutLine = Instantiate(BlockOutline, new Vector3(0, -20, 0),Quaternion.identity);
    }

    private void Awake()
    {
        BFLayerMask = LayerMask.GetMask("Block", "Floor");
        PBLayerMask = LayerMask.GetMask("PhysicsBlock");
    }

    void Update()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        blockCountText.text = $"{blockData.BlockNumber}";

        if (!KccCameraTest.togglePov)
        {
            return;
        }

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, BFLayerMask))
        {
            Vector3 pos = Hit.point;

            pos = new Vector3(
                Mathf.Floor(pos.x),
                Mathf.Floor(pos.y),
                Mathf.Floor(pos.z));

            pos += Vector3.one * 0.5f;

            NewBlockOutLine.transform.position = pos;

            if (Input.GetKey(KeyCode.F))
            {
                NewBlockOutLine.gameObject.SetActive(false);
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                NewBlockOutLine.gameObject.SetActive(true);
            }


            if (Input.GetMouseButtonDown(0))
            {
                if (blockData.BlockNumber > 0)
                {
                    if (_modelPlacementChecker.CheckValidation(pos))
                    {
                        blockData.BlockNumber -= 1;
                        blockCountText.text = $"{blockData.BlockNumber}";
                        Debug.Log("TestBlockCreateRayCastController : Valid Place!");
                        CreateBlockRpc(pos);
                    }
                    else
                    {
                        Debug.Log("TestBlockCreateRayCastController : Invalid Place!");
                    }
                }
                else
                {
                    Debug.Log("No Block!!");
                    StartCoroutine(NoBlockTextSet());
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if ( Hit.collider.name == "PhysicsBasicBlock(Clone)")
                {
                    var block = Hit.collider.GetComponent<NetworkObject>();
                    DeleteBlockRpc(block);
                    blockData.BlockNumber += 1;
                    blockCountText.text = $"{blockData.BlockNumber}";
                }
            }
        }

        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, PBLayerMask))
        {
            if (Input.GetMouseButtonDown(1))
            {
                if ( Hit.collider.name == "PhysicsBasicBlock(Clone)")
                {
                    var block = Hit.collider.GetComponent<NetworkObject>();
                    DeleteBlockRpc(block);
                    blockData.BlockNumber += 1;
                    blockCountText.text = $"{blockData.BlockNumber}";
                }
            }
        }

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void CreateBlockRpc(Vector3 pos)
    {
        var spawnObject = RunnerManager.Instance.runner.SpawnAsync(blockData.BasicBlockPrefab,
            pos, Quaternion.identity);
        spawnObject.Object.transform.SetParent(BasicBlockParent.transform);
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    private void DeleteBlockRpc(NetworkObject networkObject)
    {
        if (networkObject == null)
        {
            return;
        }
        RunnerManager.Instance.runner.Despawn(networkObject);
    }
    
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(camera.transform.position, camera.transform.forward * 99999);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Hit.point, 0.05f);
        
        Gizmos.DrawRay(Hit.point, Hit.normal);
    }*/

    IEnumerator NoBlockTextSet()
    {
        noBlockText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        noBlockText.gameObject.SetActive(false);
    }
    
}