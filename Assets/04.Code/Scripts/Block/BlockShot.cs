using System.Collections;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BlockShot : NetworkBehaviour
{
    public TMP_Text blockCountText;
    public Transform blockShotPoint;
    public Slider chargeGauge;
    public int increaseSpeed = 2;
    public BlockData blockData;
    
    private float currentGauge = 0f;
    [SerializeField] private NetworkMecanimAnimator NMAni;
    private bool isKeyPressed = false;
    public AudioSource audioThrow;
    
    [SerializeField] private Camera userCamera;
    
    
    void Start()
    {
        Debug.Log(blockData.BlockNumber);
        
    }
    
    void Update()
    {
        ThrowBlock();
    }

    private void ThrowBlock()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (KccCameraTest.togglePov)
        {
            if (Input.GetKey(KeyCode.F))
            {
                isKeyPressed = true;
                chargeGauge.gameObject.SetActive(true);
            }

            if (isKeyPressed)
            {
                if (chargeGauge.value < chargeGauge.maxValue)
                {
                    chargeGauge.value += increaseSpeed * Time.deltaTime;
                }
                else
                {
                    chargeGauge.value = chargeGauge.maxValue;
                }
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                isKeyPressed = false;
                chargeGauge.gameObject.SetActive(false);
                currentGauge = chargeGauge.value;
                chargeGauge.value = chargeGauge.minValue;
                
                if (blockData.BlockNumber > 0)
                {
                    audioThrow.Play();
                    blockData.BlockNumber -= 1;
                    blockCountText.text = $"{blockData.BlockNumber}";
                    Vector3 throwDirection = (userCamera.transform.forward * 10f) + (Vector3.up * 5f);
                    Vector3 force = throwDirection * currentGauge;
                    
                    NMAni.SetTrigger("IsThrowing");
                    
                    ThrowRpc(force);
                    
                }
                else
                {
                    Debug.Log("No Block!!");
                }
            }
        }
    }
    
    private async UniTask ThrowRpc(Vector3 force)
    {
        var blockOp = RunnerManager.Instance.runner.SpawnAsync(blockData.PhysicsBlockPrefab,
            blockShotPoint.transform.position, quaternion.identity);
        await UniTask.WaitUntil(() => blockOp.Status == NetworkSpawnStatus.Spawned);
        Rigidbody rB = blockOp.Object.GetComponent<Rigidbody>();
        
        rB.AddForce(force, ForceMode.Impulse);

       // animator.SetTrigger("IsThrowing");
    }
}