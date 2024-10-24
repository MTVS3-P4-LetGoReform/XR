using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestBlockMakerController : MonoBehaviour
{
    private TestMoveController tMoveCon;

    private Transform player;

    public TMP_Text pressText;
    public TMP_Text blockCountText;
    public Slider blockMakeGauge;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tMoveCon = GetComponent<TestMoveController>();
        player = GameObject.FindWithTag("MainCamera").transform;
        blockMakeGauge.maxValue = 20;
    }

    // Update is called once per frame
    void Update()
    {
        GetBlockFromMaker();
    }
    
    private void GetBlockFromMaker()
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

    }

   IEnumerator BlockChargeSystem()
    {
        while (blockMakeGauge.value < blockMakeGauge.maxValue)
        {
            blockMakeGauge.value += 1; // 슬라이더 값을 1씩 증가
            yield return new WaitForSeconds(1f); // 1초 대기
        }
    }

    private void BlockMakeGaugeController()
    {
        
    }
}
