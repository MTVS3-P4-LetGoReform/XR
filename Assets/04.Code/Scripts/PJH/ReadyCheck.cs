using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyCheck : MonoBehaviour
{
    public Canvas readyCanvas;
    public GameObject progressInfo;
    public TMP_Text readyText; 
    public Button gameStartButton;

    public GameObject readyButton;
    public GameObject unReadyButton;
    
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
            readyButton.SetActive(true);
            unReadyButton.SetActive(false);
            SharedGameData.Instance.RpcReady();
        }
        else
        {
            readyButton.SetActive(false);
            unReadyButton.SetActive(true);
            SharedGameData.Instance.RpcWait();
        }
    }

    public void StartGame(bool status)
    {
        SharedGameData.Instance.GameStartRpc();
    }
    
    private async UniTask Check()
    {
        var wfs = 0.5f;
        while (true)
        {
            var totalCount = RunnerManager.Instance.runner.SessionInfo.PlayerCount;
            var currentCount = SharedGameData.ReadyCount;
            readyText.text = $"현재 인원\n{currentCount}/{totalCount}";

            await UniTask.WaitForSeconds(wfs);

            if (totalCount == currentCount && SharedGameData.StartGame)
                break;
        }
        
        await UniTask.WaitForSeconds(wfs);
        readyCanvas.enabled = false;
        GameStateManager.Instance.StartGameProcess();
    }
}
