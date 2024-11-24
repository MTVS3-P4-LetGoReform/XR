using UnityEngine;

public class GoatItem : MonoBehaviour
{
    public int rank;
    public int score;
    public string username;
    public string modelName;
    public string modelImageName;
        
    public string modelId;

    public void SetGoatData (RankingEntry rankingEntry)
    {
        rank = rankingEntry.rank;
        score = rankingEntry.score;
        username = rankingEntry.username;
        modelName = rankingEntry.modelName;
        modelImageName = rankingEntry.modelImageName;

        modelId = rankingEntry.modelId;
    }
}
