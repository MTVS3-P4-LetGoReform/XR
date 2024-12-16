using UnityEngine;

public class ModelCompleteChecker : MonoBehaviour
{
    public GameObject curFloorBlocks;
    public GameObject completeFloorBlocks;
    
    private void Update()
    {
        if (GameStateManager.Instance.IsComplete())
        {
            //Debug.Log("ModelCompleteChecker : Game Completed");
            GameStateManager.Instance.DoCompleteCoroutine();
            Destroy(gameObject);
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            foreach (Transform childTrans in curFloorBlocks.transform)
            {
                childTrans.SetParent(completeFloorBlocks.transform);
            }
            //Debug.Log("ModelCompleteChecker : Block Place Trigger");
            
            GameStateManager.Instance.AddCnt(1);
        }
    }
}
