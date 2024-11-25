using UnityEngine;

public class InventoryInteraction : MonoBehaviour
{
    public Canvas StatueInventory;

    // public void Start()
    // {
    //     StatueInventory.ena;
    // }
    void Update()
    {
        ActivateStatueInventory();
    }

    public void ActivateStatueInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            StatueInventory.enabled = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StatueInventory.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
