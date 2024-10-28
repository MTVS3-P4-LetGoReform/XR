using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockShot : MonoBehaviour
{
    public GameObject ShotBlock;
    public TMP_Text blockCountText;
    public Transform blockShotPoint;
    public Slider chargeGauge;
    public int increaseSpeed = 2;
    private Camera camera;
    private float currentGauge = 0f;
    private Animator animator;
    private bool isKeyPressed = false;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(SharedBlockData.BlockNumber);
        animator = GetComponentInChildren<Animator>();
        camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();

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
            if (SharedBlockData.BlockNumber > 0)
            {
                GameObject block = Instantiate(ShotBlock, blockShotPoint.transform.position, Quaternion.identity);
                Debug.Log(block.transform.position);
                Rigidbody rB = block.GetComponent<Rigidbody>();

                Vector3 throwDirection = (camera.transform.forward * 20) + (Vector3.up * 10);
                
                rB.AddForce(throwDirection * currentGauge, ForceMode.Impulse); 
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