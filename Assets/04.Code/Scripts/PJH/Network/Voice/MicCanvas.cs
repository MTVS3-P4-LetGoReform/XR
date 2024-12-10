using System;
using Cysharp.Threading.Tasks;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class MicCanvas : MonoBehaviourSingleton<MicCanvas>
{
    public NetworkObject uIPrefab;
    public Transform playerContainer;

    public async UniTask SpawnOp()
    {
        await RunnerManager.Instance.runner.SpawnAsync(uIPrefab);
    }
}
