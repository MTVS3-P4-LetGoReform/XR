using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private ModelgenerateController _modelgenerateController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _modelgenerateController = FindObjectOfType<ModelgenerateController>();
        _modelgenerateController.GeneratePlayModel();
    }
}
