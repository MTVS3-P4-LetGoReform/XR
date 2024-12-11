using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MicInfo : NetworkBehaviour
{
    [FormerlySerializedAs("userName")] public TMP_Text TMPUserName;
    public string userName;
    
    public override void Spawned()
    {
        if (MicCanvas.Instance != null)
        {
            transform.SetParent(MicCanvas.Instance.playerContainer, false);
        }
        else
        {
            Debug.LogWarning("MicCanvas.Instance가 null값입니다.");
        }

        if (!HasStateAuthority)
        {
            Debug.Log("MicInfo - HasStateAuthority : 권한이 없습니다.");
            return;
        }

        userName = UserData.Instance.UserName;
        
        if (TMPUserName != null && UserData.Instance != null)
        {
            PlayerNameChangeRpc(userName);
        }

        Debug.Log("username : " + UserData.Instance.UserName);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void PlayerNameChangeRpc(string username)
    {
        TMPUserName.text = username;
    }
    
}
