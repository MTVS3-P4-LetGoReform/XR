using System.Collections;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    public Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerTransform == null)
        {
            return;
        }
        else
        {
            GetTreasureResult();
        }
    }

    private void GetTreasureResult()
    {
        
        
        
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
