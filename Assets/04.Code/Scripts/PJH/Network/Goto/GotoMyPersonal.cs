using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class GotoMyPersonal : MonoBehaviour
{
    public Button socialButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        socialButton.onClick.AddListener(GoToMyPersonalScene);
    }

    // Update is called once per frame
    private async void GoToMyPersonalScene()
    {

        Debug.Log("userid: " + UserData.Instance.UserId);
        var args = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = UserData.Instance.UserId,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "UserId", UserData.Instance.UserId },
            }
        };
        await RunnerManager.Instance.ShutdownRunner();
        await RunnerManager.Instance.RunnerStart(args, 3);
    }
}
