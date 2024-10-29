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
    }

    public void ChooseStatueOnClick()
    {
        Vector3 point = new Vector3(5, 26, -17);
        Instantiate(minionsPrefab, point, Quaternion.identity);
    }

    public void CloseButtonOnClick()
    {
        inventory.gameObject.SetActive(false);
    }
}
