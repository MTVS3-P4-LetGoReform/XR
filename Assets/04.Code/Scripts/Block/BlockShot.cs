using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockShot : NetworkBehaviour
{
    public GameObject ShotBlock;
    public TMP_Text blockCountText;
    public Transform blockShotPoint;
    public Slider chargeGauge;
    public int increaseSpeed = 2;
    public BlockData blockData;
    public List<BlockData> blockDataList;
    private Camera camera;
    private float currentGauge = 0f;
    private Animator animator;
    private bool isKeyPressed = false;
    
    void Start()
    {
        Debug.Log(blockData.BlockNumber);
        animator = GetComponentInChildren<Animator>();
        camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();

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
                    blockData.BlockNumber -= 1;
                    blockCountText.text = $"{blockData.BlockNumber}";
                    ThrowRpc();
                }
                else
                {
                    Debug.Log("No Block!!");
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void ThrowRpc()
    {
        GameObject block =
            Instantiate(blockData.PhysicsBlockPrefab, blockShotPoint.transform.position, Quaternion.identity);

        Debug.Log(block.transform.position);
        Rigidbody rB = block.GetComponent<Rigidbody>();

        Vector3 throwDirection = (camera.transform.forward * 20f) + (Vector3.up * 10f);

        rB.AddForce(throwDirection * currentGauge, ForceMode.Impulse);
        Debug.Log(currentGauge);

        animator.SetTrigger("IsThrowing");
    }
}