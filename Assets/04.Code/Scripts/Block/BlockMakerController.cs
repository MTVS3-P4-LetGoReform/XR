using System;
using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockMakerController : NetworkBehaviour
{
    private TestMoveController tMoveCon;

   // private Transform player;
    private float blockChargeSpeed = 1f;
    public Transform playerTransform;
    public TMP_Text pressText;
    public Slider blockMakeGauge;
    
    void Start()
    {
        if (!HasStateAuthority)
        {
            Destroy(gameObject);
        }
        
        GameObject textObject = GameObject.Find("Press_E_Text");
        if (textObject != null)
        {
            pressText = textObject.GetComponent<TMP_Text>();
            pressText.gameObject.SetActive(false);
        }
        
        tMoveCon = GetComponent<TestMoveController>();
       // player = GameObject.Find("PlayerCamera").transform;
        
        blockMakeGauge.maxValue = 20;
        StartCoroutine(FindPlayer());
    }
    
    void LateUpdate()
    {
        if (playerTransform == null)
        {
            return;
        }
        else
        {
            GetBlockFromMaker();
        }
    }
    
    private void GetBlockFromMaker()
    {
        
        if (Vector3.Distance(transform.position, playerTransform.transform.position) < 5)
        {
            pressText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (blockMakeGauge.value >= 20)
                {
                    blockMakeGauge.value = 0f;
                    SharedBlockData.BlockNumber = 20;
                    StartCoroutine(BlockChargeSystem());
                }
            }
        }
        else
        {
            pressText.gameObject.SetActive(false);
        }

    } 

    IEnumerator BlockChargeSystem()
    {
        while (blockMakeGauge.value < blockMakeGauge.maxValue)
        {
            blockMakeGauge.value += blockChargeSpeed * Time.deltaTime; // 슬라이더 값을 deltaTime에 맞게 증가
            yield return null; // 1초 대기
        }
    }

    private IEnumerator FindPlayer()
    {
        GameObject playerObject = null;

        while (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");

            if (playerObject == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        playerTransform = playerObject.transform;
        
    }
}
