using UnityEngine;

public class SoundController : MonoBehaviour
{
    public  AudioSource bgmAudioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            bgmAudioSource.Stop();
        }
    }
}
