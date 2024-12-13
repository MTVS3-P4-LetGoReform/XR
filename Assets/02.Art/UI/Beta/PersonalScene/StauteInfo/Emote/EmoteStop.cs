using UnityEngine;
using UnityEngine.Video;

public class EmoteStop : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public GameObject model;
    public GameObject image;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("비디오 재생이 끝났습니다.");
        model.SetActive(true);
        image.SetActive(false);
    }
}
