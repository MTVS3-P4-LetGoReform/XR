using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ReadyCheck : MonoBehaviour
{
    public Canvas readyCanvas;
    public GameObject progressInfo;
    public TMP_Text readyText; 
    public Button gameStartButton;

    public GameStateManager _gameStateManager;
    
    private void Start()
    {
        Check().Forget();
        PlayerInput.OnPlayerReady += Ready;
        PlayerInput.OnGameStart += StartGame;
    }

    private void Ready(bool status)
    {
        if (status)
        {
            SharedGameData.Instance.RpcReady();
        }
        else
        {
            SharedGameData.Instance.RpcWait();
        }
    }

    public void StartGame(bool status)
    {
        SharedGameData.Instance.GameStartRpc();
        GameStateManager.Instance.StartGameProcess();
    }
    
    private async UniTask Check()
    {
        var wfs = 0.5f;
        while (true)
        {
            var totalCount = RunnerManager.Instance.runner.SessionInfo.PlayerCount;
            var currentCount = SharedGameData.ReadyCount;
            readyText.text = $"준비\n{currentCount}/{totalCount}";

            await UniTask.WaitForSeconds(wfs);

            if (totalCount == currentCount && SharedGameData.StartGame)
                break;
        }
        
        await UniTask.WaitForSeconds(wfs);
        readyCanvas.enabled = false;
    }
}
