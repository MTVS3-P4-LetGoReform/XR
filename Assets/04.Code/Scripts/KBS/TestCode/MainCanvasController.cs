using System.Collections;
using UnityEngine;

public class MainCanvasController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float duration = 1f; // 페이드 인 시간

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // 마지막에 alpha를 1로 고정
    }
}
