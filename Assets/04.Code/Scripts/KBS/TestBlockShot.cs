using TMPro;
using UnityEngine;

public class TestBlockShot : MonoBehaviour
{
    public GameObject ShotBlock;

    public float throwPower = 1f;

    public TMP_Text blockCountText;
    public Transform playerShot;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(SharedBlockData.BlockNumber);
        
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (SharedBlockData.BlockNumber > 0)
            {
                GameObject block = Instantiate(ShotBlock, playerShot.transform.position, Quaternion.identity); 
                Debug.Log(block.transform.position);
                Rigidbody rB = block.GetComponent<Rigidbody>();
                rB.AddForce(Camera.main.transform.position * throwPower, ForceMode.Impulse);

                SharedBlockData.BlockNumber -= 1;
                blockCountText.text = $"{SharedBlockData.BlockNumber}";

            }
            else
            {
                Debug.Log("No Block!!");
            }
            
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (SharedBlockData.BlockNumber > 0)
            {
                GameObject block = Instantiate(ShotBlock, playerShot.transform.position, Quaternion.identity); 
                Debug.Log(block.transform.position);
                Rigidbody rB = block.GetComponent<Rigidbody>();
                rB.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);

                SharedBlockData.BlockNumber -= 1;
                blockCountText.text = $"{SharedBlockData.BlockNumber}";

            }
            else
            {
                Debug.Log("No Block!!");
            }
        }
        
    }
}
