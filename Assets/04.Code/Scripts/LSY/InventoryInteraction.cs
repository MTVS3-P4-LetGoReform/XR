using UnityEngine;

public class InventoryInteraction : MonoBehaviour
{
    public GameObject StatueInventory;

    public void Start()
    {
        StatueInventory.SetActive(false);
    }
    void Update()
    {
        ActivateStatueInventory();
    }

    public void ActivateStatueInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            StatueInventory.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StatueInventory.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
