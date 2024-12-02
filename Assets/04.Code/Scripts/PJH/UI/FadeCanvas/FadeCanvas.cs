using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup.alpha = 0f;
    }

    public void FadeIn()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.2f).SetUpdate(true);
    }
}
