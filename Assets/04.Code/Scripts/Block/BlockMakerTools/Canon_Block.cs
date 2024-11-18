using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Canon_Block : MonoBehaviour
{
    public Transform playerTransform;
    public Transform playerCanonPoint;
    public Transform canonShotPoint;
    public TMP_Text rideOnCanonText;
    private Camera playerCamera;
    
    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < 5)
        {
            rideOnCanonText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                rideOnCanonText.gameObject.SetActive(false);
                CanonMode();
            }
        }
    }

    private void CanonMode()
    {
        playerTransform.position = playerCanonPoint.position;
        
        
    }

    
    private IEnumerator FindPlayer()
    {
        GameObject playerObject = null;

        while (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");

            if (playerObject == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        playerTransform = playerObject.transform;
        
    }
}
