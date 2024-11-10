using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : NetworkBehaviour,IPlayerJoined
{
    [Networked, OnChangedRender(nameof(OnMasterClientChanged))]
    public NetworkBool IsMasterClient { get; set; }

    public Image masterClientIcon;

    public override void Spawned()
    {
        base.Spawned();
        
        if (HasStateAuthority)
        {
            IsMasterClient = Runner.IsSharedModeMasterClient;
        }
        masterClientIcon.enabled = IsMasterClient;
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
        masterClientIcon.enabled = IsMasterClient;
    }
}
