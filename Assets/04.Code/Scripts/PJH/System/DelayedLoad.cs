using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DelayedLoad : MonoBehaviour
{
    public float delayTime = 1f;

    private async void Start()
    {
        Debug.Log("로딩 대기중");

        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        
        Debug.Log("로딩 완료");

        SceneLoadManager.isLoaded = true;
    }
}
