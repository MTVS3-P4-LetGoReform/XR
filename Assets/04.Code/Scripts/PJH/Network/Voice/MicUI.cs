using Fusion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MicUI : MonoBehaviour,IPlayerJoined
{
    private NetworkObject _playerImage;
    private Transform _parent;
    
    public void PlayerJoined(PlayerRef player)
    {
        var spawnObject = RunnerManager.Instance.runner.Spawn(_playerImage,Vector3.zero,quaternion.identity);
        spawnObject.transform.SetParent(_parent);
    }
}
