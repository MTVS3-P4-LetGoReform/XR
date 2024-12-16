using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedLoad : MonoBehaviour
{
    private float delayTime = 2f;

    private async void Awake()
    {
        Debug.Log("로딩 대기중");

        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        
        Debug.Log("로딩 완료");

        SceneLoadManager.isLoaded = true;
    }
}
