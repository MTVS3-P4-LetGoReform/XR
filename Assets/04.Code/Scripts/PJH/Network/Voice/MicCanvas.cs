using System;
using Cysharp.Threading.Tasks;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class MicCanvas : MonoBehaviour
{
    public NetworkObject uIPrefab;
    public Transform parent;

    public async UniTask SpawnOp()
    {
        var spawnObject = await RunnerManager.Instance.runner.SpawnAsync(uIPrefab,Vector3.zero,quaternion.identity);
        spawnObject.transform.SetParent(parent);
    }
}
