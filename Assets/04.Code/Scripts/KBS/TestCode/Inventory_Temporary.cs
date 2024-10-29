using UnityEngine;
using UnityEngine.UI;

public class Inventory_temporary : MonoBehaviour
{
    public Image inventory;
    
    private bool isInvenPush = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.gameObject.SetActive(true);
        }
    }
}
