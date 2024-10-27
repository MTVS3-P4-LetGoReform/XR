using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockMakerController : MonoBehaviour
{
    private TestMoveController tMoveCon;

   // private Transform player;
    private float blockChargeSpeed = 1f;
    public TMP_Text pressText;
    //public TMP_Text blockCountText;
    public Slider blockMakeGauge;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tMoveCon = GetComponent<TestMoveController>();
       // player = GameObject.Find("PlayerCamera").transform;
        
        blockMakeGauge.maxValue = 20;
    }

    // Update is called once per frame
    void Update()
    {
        //GetBlockFromMaker();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pressText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (blockMakeGauge.value >= 20)
                {
                    blockMakeGauge.value = 0f;
                    SharedBlockData.BlockNumber = 20;
                    Debug.Log(SharedBlockData.BlockNumber);
                    StartCoroutine(BlockChargeSystem());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pressText.gameObject.SetActive(false);
        }
    }

   /* private void GetBlockFromMaker()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 5)
        {
            pressText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (blockMakeGauge.value >= 20)
                {
                    blockMakeGauge.value = 0f;
                    SharedBlockData.BlockNumber = 20;
                    blockCountText.text = ($"{SharedBlockData.BlockNumber}");
                    StartCoroutine(BlockChargeSystem());
                }
            }
        }
        else
        {
            pressText.gameObject.SetActive(false);
        }

    } */

    IEnumerator BlockChargeSystem()
    {
        while (blockMakeGauge.value < blockMakeGauge.maxValue)
        {
            blockMakeGauge.value += blockChargeSpeed * Time.deltaTime; // 슬라이더 값을 deltaTime에 맞게 증가
            yield return null; // 1초 대기
        }
    }

    private void BlockMakeGaugeController()
    {
        
    }
}
