using UnityEngine;
using UnityEngine.UI;

public class Inventory_temporary : MonoBehaviour
{
    public Image inventory;
    public Button closeButton;
    public GameObject minionsPrefab;
    
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector3 point = new Vector3(19f, 27.23f, 15f);
            Instantiate(minionsPrefab, point, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inventory.gameObject.SetActive(false);
        }
    }
}
