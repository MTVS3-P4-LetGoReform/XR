using System;
using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BlockmakerController : NetworkBehaviour
{
    private TestMoveController tMoveCon;

    // private Transform player;
    private float blockChargeSpeed = 1f;
    private Transform playerTransform;
    public TMP_Text pressText;
    
    void Start()
    {
        if (!HasStateAuthority)
        {
            Destroy(gameObject);
        }
        
        GameObject textObject = GameObject.Find("Press_E_Text");
        if (textObject != null)
        {
            pressText = textObject.GetComponent<TMP_Text>();
            pressText.gameObject.SetActive(false);
        }
        
        tMoveCon = GetComponent<TestMoveController>();
        
        StartCoroutine(FindPlayer());
    }
    
    void LateUpdate()
    {
        if (playerTransform == null)
        {
            return;
        }
        else
        {
            GetBlockFromMaker();
        }
    }
    
    private void GetBlockFromMaker()
    {
        
        if (Vector3.Distance(transform.position, playerTransform.transform.position) < 5)
        {
            pressText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                // UI 캔버스 띄윅
            }
        }
        else
        {
            pressText.gameObject.SetActive(false);
        }

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