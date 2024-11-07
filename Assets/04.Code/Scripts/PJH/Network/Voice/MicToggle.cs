using Photon.Voice.Unity;
using UnityEngine;

public class MicToggle : MonoBehaviour
{
    public Recorder recorder;
    public Speaker speaker;
    
    public void ToggleMic()
    {
        recorder.TransmitEnabled = false;
        var ispl = speaker.IsPlaying;
    }
}
