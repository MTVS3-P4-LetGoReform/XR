using System;
using UnityEngine;
using UnityEngine.UI;

public class RewordCanvas : MonoBehaviour
{
    public Canvas masterCanvas;
    public Button masterRewordButton;
    
    public Canvas userCanvas;
    public Button userRewordButton;

    private void Start()
    {
        masterRewordButton.onClick.AddListener(MasterReword);
        userRewordButton.onClick.AddListener(UserReword);
    }

    private async void MasterReword()
    {
        UserData.Instance.UserId;
        Debug.Log("보상획득: 스태츄");
        Cursor.lockState = CursorLockMode.Locked;
        await RunnerManager.Instance.JoinPublicSession();
    }
    
    private async void UserReword()
    {
        Debug.Log("보상획득: 크레딧");
        Cursor.lockState = CursorLockMode.Locked;
        await RunnerManager.Instance.JoinPublicSession();
    }
}
