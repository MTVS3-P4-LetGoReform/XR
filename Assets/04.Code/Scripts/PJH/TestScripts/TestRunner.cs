using System;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class TestRunner : MonoBehaviour
{
    public NetworkRunner Runner;
    [FormerlySerializedAs("micUI")] public MicCanvas micCanvas;

    private void Start()
    {
        FirstStartGame().Forget();
        micCanvas = FindAnyObjectByType<MicCanvas>();
    }

    private async UniTask FirstStartGame()
    {
      
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "RunnerTest"
        };

        var result = await Runner.StartGame(startArgs);
        if (result.Ok)
        {
            micCanvas.SpawnOp();
        }
    }
}
