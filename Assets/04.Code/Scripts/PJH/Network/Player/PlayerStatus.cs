using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : NetworkBehaviour,IPlayerJoined,IPlayerLeft
{
    [Networked, OnChangedRender(nameof(OnMasterClientChanged))]
    public NetworkBool IsMasterClient { get; set; }
    private UserInfoCanvas _userInfo;
    
    public string playerName;
    
    public override void Spawned()
    {
        base.Spawned();
        if (!HasStateAuthority)
            return;
        
        IsMasterClient = Runner.IsSharedModeMasterClient;
        Debug.Log("마스터 클라이언트 여부 :"+IsMasterClient);
        
        UserData.ChangeName += OnChangedUserName;
        
        playerName = UserData.Instance.UserName;
        
        FindUserInfoCanvas();

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            FindReadyCanvas();
        }
    }

    private void OnChangedUserName()
    {
        playerName = UserData.Instance.UserName;
        
        FindUserInfoCanvas();
    }

    private void FindUserInfoCanvas()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            _userInfo = FindAnyObjectByType<UserInfoCanvas>();
            _userInfo.canvas.enabled = true;
            
            SetUserName();
        }
    }

    private void SetUserName()
    {
        _userInfo.username.text = playerName;
        
        Debug.Log("이름 변경 : " + playerName);
    }

    private void FindReadyCanvas()
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

    public void Reword(bool complete)
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
    
    public void PlayerLeft(PlayerRef player)
    {
        IsMasterClient = RunnerManager.Instance.runner.IsSharedModeMasterClient;
    }
    
    void OnMasterClientChanged()
    {
        //masterClientIcon.enabled = IsMasterClient;
    }
}
