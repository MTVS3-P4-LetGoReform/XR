using Fusion;
using Photon.Voice.Unity;
using UnityEngine;

public class MicHighlight : NetworkBehaviour
{
    public GameObject micImage;
    public GameObject highLightImage;
    
    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        micImage.SetActive(true);
        highLightImage.SetActive(false);
        
        PlayerInput.MicMute += MicActiveRpc;
        MicController.IsPlaying += SpeakingHighlightRpc;
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    private void MicActiveRpc(bool active)
    {   
        micImage.SetActive(!active);
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    private void SpeakingHighlightRpc(bool active)
    {
        highLightImage.SetActive(active);
    }
}
