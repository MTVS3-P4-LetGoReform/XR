using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BlockCreateRaycastController : NetworkBehaviour
{
    public GameObject BlockPrefab;
    public GameObject BlockOutline;
    public GameObject NewBlockOutLine;
    public TMP_Text noBlockText;
    public TMP_Text blockCountText;
    public Image imageBlock;
    public LayerMask BFLayerMask;
    public LayerMask PBLayerMask;
    public LayerMask BMLayerMask;
    public BlockData blockData;
    public AudioSource audioDropBox;
    
    private RaycastHit Hit;
    private GameObject _basicBlockParent;
    [SerializeField] private Camera userCamera;
    [SerializeField] private NetworkMecanimAnimator NMAni;

    private ModelPlacementChecker _modelPlacementChecker;

    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        _modelPlacementChecker = FindObjectOfType<ModelPlacementChecker>();
        
        NewBlockOutLine = Instantiate(BlockOutline, new Vector3(0, -20, 0),Quaternion.identity);
        _basicBlockParent = Instantiate(new GameObject());
        _basicBlockParent.name = "BasicBlockParent";
    }

    private void Awake()
    {
        BFLayerMask = LayerMask.GetMask("Block", "Floor");
        PBLayerMask = LayerMask.GetMask("PhysicsBlock");
    }

    private void Start()
    {
        blockCountText.gameObject.SetActive(false);
       
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

        Ray ray = new Ray(userCamera.transform.position, userCamera.transform.forward);
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
                        audioDropBox.Play();
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
                if (Hit.collider != null && Hit.collider.name == "PhysicsBasicBlock(Clone)")
                {
                    var block = Hit.collider.GetComponent<NetworkObject>();
                    Debug.Log("위쪽 실행");
                    // 블록이 삭제 요청 중인지 확인
                    if (block != null && !_pendingDeletionBlocks.Contains(block))
                    {
                        // 삭제 요청 중인 블록으로 추가
                        _pendingDeletionBlocks.Add(block);

                        RequestDeleteBlockRpc(block);
                        
                        blockData.BlockNumber += 1;
                        blockCountText.text = $"{blockData.BlockNumber}";
                    }
                    else
                    {
                        Debug.LogWarning("Block is already pending deletion.");
                    }
                }
            }
        }

        if (Physics.Raycast(ray, out Hit, Mathf.Infinity,BMLayerMask))
        {
            if (Hit.collider != null && Hit.collider.name == "BlockMaker")
            {
                imageBlock.gameObject.SetActive(true);
                blockCountText.gameObject.SetActive(true);
            }
        }

        if (Physics.Raycast(ray, out Hit, Mathf.Infinity, PBLayerMask))
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (Hit.collider != null && Hit.collider.name == "PhysicsBasicBlock(Clone)")
                {
                    var block = Hit.collider.GetComponent<NetworkObject>();
                    
                    NMAni.SetTrigger("IsPickUp");
            
                    // 블록이 삭제 요청 중인지 확인
                    if (block != null && !_pendingDeletionBlocks.Contains(block))
                    {
                        // 삭제 요청 중인 블록으로 추가
                        OnBlockDeletionAddRpc(block);

                        // 블록 삭제 RPC 호출
                        RequestDeleteBlockRpc(block);
                    }
                    else
                    {
                        Debug.LogWarning("Block is already pending deletion.");
                    }
                }
            }
        }

    }
    
    private HashSet<NetworkObject> _pendingDeletionBlocks = new HashSet<NetworkObject>();

    //SpawnAsync를 쓰기 때문에 Rpc사용 XX
    private async void CreateBlockRpc(Vector3 pos)
    {
        if (RunnerManager.Instance.runner != null)
        {
            var spawnObject = await RunnerManager.Instance.runner.SpawnAsync(
                blockData.BasicBlockPrefab, 
                pos, 
                Quaternion.identity
            );
            
            SharedGameData.Instance.BlockCountRpc();
            
            // 이름 설정
            spawnObject.name = $"BasicBlock : {SharedGameData.BlockCount}";
            
            // 부모 설정
            if (spawnObject != null && _basicBlockParent != null)
            {
                spawnObject.transform.SetParent(_basicBlockParent.transform,true);
            }
            else
            {
                Debug.Log($"SpawnObject : {spawnObject.name} or _basicBlockParent : {_basicBlockParent.name}= null");
            }
            
        }
        else
        {
            Debug.LogError("Runner is null. Cannot spawn block.");
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RequestDeleteBlockRpc(NetworkObject networkObject)
    {
        // 오브젝트가 유효한지 확인
        if (networkObject == null || !networkObject.IsValid)
        {
            Debug.LogWarning("NetworkObject is invalid.");
            return;
        }

        // 오브젝트의 소유자에게 삭제 요청
        if (networkObject.HasStateAuthority)
        {
            DeleteBlock(networkObject);
        }
        else
        {
            // 소유자가 아닌 경우, 소유자에게 권한 요청
            networkObject.RequestStateAuthority();
            // 권한을 받은 후에 삭제 시도
            if (networkObject.HasStateAuthority)
            {
                DeleteBlock(networkObject);
            }
            else
            {
                Debug.LogWarning("Cannot delete block - No authority granted");
            }
        }
    }


    private void DeleteBlock(NetworkObject networkObject)
    {
        if (RunnerManager.Instance.runner == null)
        {
            Debug.LogError("Runner is null. Cannot despawn block.");
            return;
        }

        // 삭제 시작을 알림
        OnBlockDeletionAddRpc(networkObject);
    
        // Despawn 실행
        RunnerManager.Instance.runner.Despawn(networkObject);
    
        // 삭제 완료를 알림
        OnBlockDeletionCompletedRpc(networkObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void OnBlockDeletionAddRpc(NetworkObject block)
    {
        if (block != null)
        {
            _pendingDeletionBlocks.Add(block);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void OnBlockDeletionCompletedRpc(NetworkObject block)
    {
        if (block != null && _pendingDeletionBlocks.Contains(block))
        {
            _pendingDeletionBlocks.Remove(block);

            if (HasStateAuthority)
            {
                AddBlockCount();
            }
            else
            {
                Debug.Log("BlockCreateRaycastController - OnBlockDeletionCompletedRpc : 권한이 없습니다.");
            }
        }
    }

    private void AddBlockCount()
    {
        // 블록 번호 증가
        blockData.BlockNumber += 1;
        blockCountText.text = $"{blockData.BlockNumber}";
    }
    
    IEnumerator NoBlockTextSet()
    {
        noBlockText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        noBlockText.gameObject.SetActive(false);
    }
}