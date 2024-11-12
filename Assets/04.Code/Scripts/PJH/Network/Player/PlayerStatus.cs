using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStatus : NetworkBehaviour,IPlayerJoined
{
    [Networked, OnChangedRender(nameof(OnMasterClientChanged))]
    public NetworkBool IsMasterClient { get; set; }
    
    
    public override void Spawned()
    {
        base.Spawned();

        if (!HasStateAuthority)
            return;
        
        IsMasterClient = Runner.IsSharedModeMasterClient;
        Debug.Log("마스터 클라이언트 여부 :"+IsMasterClient);

        if (IsMasterClient && SceneManager.GetActiveScene().buildIndex == 2)
        {
            var readyCheck = FindAnyObjectByType<ReadyCheck>();
            readyCheck.gameStartButton.gameObject.SetActive(true);

            GameStateManager.Instance.Complete += Reword;
        }
        
        if (SceneManager.GetActiveScene().buildIndex == 1 && SceneManager.GetActiveScene().buildIndex == 3)
        {
            UserInfoCanvas userInfo = FindAnyObjectByType<UserInfoCanvas>();
            if (userInfo == null)
            {
                Debug.LogError("UserInfoCanvas 객체를 찾을 수 없습니다.");
                return;
            }

            if (UserData.Instance.UserName == null)
            {
                Debug.LogError("UserData를 찾을 수 없습니다.");
            }
            userInfo.canvas.enabled = true;
            userInfo.username.text = UserData.Instance.UserName;
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            ReadyCheck readyCheck = FindAnyObjectByType<ReadyCheck>();
            readyCheck.readyCanvas.enabled = true;
            readyCheck.progressInfo.SetActive(true);
        }
    }

    private void Reword(bool complete)
    {
        var rewordCanvas = FindAnyObjectByType<RewordCanvas>();
        if (IsMasterClient)
        {
            rewordCanvas.masterCanvas.gameObject.SetActive(complete);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            rewordCanvas.userCanvas.gameObject.SetActive(complete);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (HasStateAuthority)
            IsMasterClient = runner.IsSharedModeMasterClient;
    }

    public void PlayerJoined(PlayerRef player)
    {
        IsMasterClient = RunnerManager.Instance.runner.IsSharedModeMasterClient;
    }
    
    void OnMasterClientChanged()
    {
        //masterClientIcon.enabled = IsMasterClient;
    }
}
