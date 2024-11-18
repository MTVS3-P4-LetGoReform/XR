using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestBlockShot : MonoBehaviour
{
    public GameObject ShotBlock;
    public TMP_Text blockCountText;
    public Transform blockShotPoint;
    public Slider chargeGauge;
    public int increaseSpeed = 2;
    public Canvas throwCanvas;
    private float currentGauge = 0f;
    private Animator animator;
    private bool isKeyPressed = false;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(SharedBlockData.BlockNumber);
        animator = GetComponentInParent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        ThrowBlock();
    }

    private void ThrowBlock()
    {
        if (Input.GetKey(KeyCode.F))
        {
            isKeyPressed = true;
            throwCanvas.gameObject.SetActive(true);
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
            throwCanvas.gameObject.SetActive(false);
            currentGauge = chargeGauge.value;
            chargeGauge.value = chargeGauge.minValue;
            if (SharedBlockData.BlockNumber > 0)
            {
                GameObject block = Instantiate(ShotBlock, blockShotPoint.transform.position, Quaternion.identity);
                Debug.Log(block.transform.position);
                Rigidbody rB = block.GetComponent<Rigidbody>();
                
                rB.AddForce(Camera.main.transform.forward * currentGauge, ForceMode.Impulse); 
                Debug.Log(currentGauge);
                
                SharedBlockData.BlockNumber -= 1;
                blockCountText.text = $"{SharedBlockData.BlockNumber}";

                animator.SetTrigger("IsThrowing");

            }
            else
            {
                Debug.Log("No Block!!");
            }
        }
    }
    
    
}

