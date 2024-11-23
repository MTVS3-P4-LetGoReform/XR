using System;
using UnityEngine;
using UnityEngine.UI;

public class GoatItem : MonoBehaviour
{
    public int rank;
    public int score;
    public string username;
    public string modelName;
    public string modelImageName;
        
    public string modelId;

    public Button likeButton;

    public void SetGoatData (RankingEntry rankingEntry , Action<string> onAccept)
    {
        rank = rankingEntry.rank; // int 
        score = rankingEntry.score; // int
        username = rankingEntry.username; // string
        modelName = rankingEntry.modelName; // string
        modelImageName = rankingEntry.modelImageName; // string

        modelId = rankingEntry.modelId;
        likeButton.onClick.AddListener(() => onAccept(modelId));
    }
}
