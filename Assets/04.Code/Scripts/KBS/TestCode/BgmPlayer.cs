using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    public AudioSource audioSourceBGM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSourceBGM.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
