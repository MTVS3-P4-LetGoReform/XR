using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float duration = 0.15f;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
    }

    public async UniTask Show()
    {
        canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        await canvasGroup.DOFade(1f, duration);
    }

    public async UniTask Hide()
    {
        await canvasGroup.DOFade(0f, duration);
        canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
    }
}