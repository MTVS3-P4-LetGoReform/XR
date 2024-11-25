using System;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class MicCanvas : MonoBehaviour
{
    public NetworkObject uIPrefab;
    public Transform parent;
    public GameObject micUI;

    private void Start()
    {
        PlayerInput.MicMute += MicActive;
    }

    private void MicActive(bool active)
    {
        micUI.SetActive(active);
    }

    public void SpawnOp()
    {
        Debug.Log("입장이요");
        var spawnObject = RunnerManager.Instance.runner.SpawnAsync(uIPrefab,Vector3.zero,quaternion.identity);
        Debug.Log(spawnObject);
        spawnObject.Object.transform.SetParent(parent);
    }
}
