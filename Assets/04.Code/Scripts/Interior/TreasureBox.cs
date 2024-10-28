using System.Collections;
using TMPro;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    public Transform playerTransform;
    public TMP_Text pressEText;
    
    void Start()
    {
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
            GetTreasureResult();
        }
    }

    private void GetTreasureResult()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < 5f)
        {
            pressEText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                //보상 스태츄를 얻는 부분
                pressEText.gameObject.SetActive(false);
            }
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
