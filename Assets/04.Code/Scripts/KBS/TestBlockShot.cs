using TMPro;
using UnityEngine;

public class TestBlockShot : MonoBehaviour
{
    public GameObject ShotBlock;

    public float throwPower = 10f;

    public TMP_Text blockCountText;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(SharedBlockData.BlockNumber);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (SharedBlockData.BlockNumber > 0)
            {
                GameObject block = Instantiate(ShotBlock);
                block.transform.position = Camera.main.transform.forward;
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
