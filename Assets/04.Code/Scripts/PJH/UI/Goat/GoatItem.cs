using System;
using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GoatItem : MonoBehaviour
{
    public WebApiData webApiData;
    public DebugModeData debugModeData;
    
    public Button likeButton;
    public Image modelImage;
    
    public TMP_Text modelName;
    public TMP_Text userName;
    public TMP_Text score;
    
    public RankingEntry RankingEntry;
    
    [Header("Data")]
    private int _rank;
    public string selectImageName;
    private string _modelId;
    
    

    public void SetGoatData (RankingEntry rankingEntry , Action<string> onAccept)
    {
        //UI
        score.text = rankingEntry.score.ToString(); // int
        userName.text = rankingEntry.username; // string
        modelName.text = rankingEntry.modelName; // string
        if (string.IsNullOrEmpty(modelName.text))
        {
            modelName.text = "말랑 햄스터";
            Debug.LogWarning("modelName = Null");
        }
        //SetImage(rankingEntry.selectImageName);
        
        // Data
        _rank = rankingEntry.rank; // int 
        RankingEntry = rankingEntry;
        selectImageName = rankingEntry.selectImageName; // string
        _modelId = rankingEntry.modelId;
        
        likeButton.onClick.AddListener(() => onAccept(_modelId));
    }
}
