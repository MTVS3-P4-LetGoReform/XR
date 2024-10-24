using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestBlockShot : MonoBehaviour
{
    public GameObject ShotBlock;

    public int[] throwPower = {1,2,3,4,5,6,7,8,9,10};

    public TMP_Text blockCountText;
    public Transform blockShotPoint;
    public Slider chargeGauge;
    public int increaseSpeed = 2;
    private float currentGauge = 0f;
    private Animator animator;
    private bool isKeyPressed = false;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(SharedBlockData.BlockNumber);
        
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponentInChildren<Animator>();

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

