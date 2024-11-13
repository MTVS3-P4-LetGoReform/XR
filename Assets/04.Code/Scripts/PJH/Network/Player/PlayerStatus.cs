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
        
       
        
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            ReadyCheck readyCheck = FindAnyObjectByType<ReadyCheck>();
            readyCheck.readyCanvas.enabled = true;
            readyCheck.progressInfo.SetActive(true);
            GameStateManager.Instance.Complete += Reword;
            if (IsMasterClient)
            {
                readyCheck.gameStartButton.gameObject.SetActive(true);
            }

            BlockMakerController blockMakerController = FindAnyObjectByType<BlockMakerController>();
            blockMakerController.playerTransform = gameObject.transform;
        }
        
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            UserInfoCanvas userInfo = FindAnyObjectByType<UserInfoCanvas>();
            userInfo.canvas.enabled = true;
            userInfo.username.text = UserData.Instance.UserName;
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
