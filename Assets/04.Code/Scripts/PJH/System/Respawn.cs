using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어 맵밖으로 떨어졌을 시 리스폰
public class Respawn : MonoBehaviour
{
    public Transform[] respawnPoint;
    private Transform _currentRespawnPoint;

    private NetworkObject _playerNetworkObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var ntObject = other.GetComponent<NetworkObject>();
            if (ntObject.InputAuthority != RunnerManager.Instance.runner.LocalPlayer)
                return;
            
            _playerNetworkObject = ntObject;
            Teleport();
        }
    }

    private void Teleport()
    {
        var ntNetworkCharacterController = _playerNetworkObject.GetComponent<NetworkCharacterController>();
        _currentRespawnPoint = SceneManager.GetActiveScene().buildIndex switch
        {
            1 => respawnPoint[1],
            2 => respawnPoint[2],
            3 => respawnPoint[3],
            _ => respawnPoint[1] // default case
        };
        
        ntNetworkCharacterController.Teleport(_currentRespawnPoint.position);
    }
}
