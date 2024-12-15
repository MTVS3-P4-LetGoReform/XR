using System.Collections;
using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private StatueInventoryController inventoryController;

    void Start()
    {
        StartCoroutine(FindInventoryController());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FindInventoryController()
    {
        while (inventoryController == null)
        {
            inventoryController = FindObjectOfType<StatueInventoryController>();
            yield return null;
        }
    }

    public void InventoryTestBtn()
    {
        inventoryController.StatueInvenTestBtn();
    }

}
