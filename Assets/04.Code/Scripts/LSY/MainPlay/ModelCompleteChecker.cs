using UnityEngine;

public class ModelCompleteChecker : MonoBehaviour
{
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
            //Debug.Log("ModelCompleteChecker : Block Place Trigger");
            GameStateManager.Instance.AddCnt(1);
        }
    }
}
