using UnityEngine;

public class SoundBox : MonoBehaviour
{
    
    public AudioSource audioSourceButton;
    public AudioSource audioSourceThrow;
    public AudioSource audioSourceDropBlock;
    
    void Start()
    {
        
    }
    

    public void ButtonSoundOnClick()
    {
        audioSourceButton.Play();
    }

    public void ThrowSoundOnClick()
    {
        audioSourceThrow.Play();
    }

    public void DropSoundOnClick()
    {
        audioSourceDropBlock.Play();
    }
    
}
