using System;
using UnityEngine;

public class TestTriggerEvent : MonoBehaviour
{

    public GameObject twoBlockLine;

    private TestBlockCreateRayCastController TBCRCC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TBCRCC = GetComponent<TestBlockCreateRayCastController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other)
    {
        
    }
}
