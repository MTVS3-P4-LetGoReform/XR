using System;
using Fusion;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using UnityEngine;

public class MicController : NetworkBehaviour
{
    public static Action<bool> IsPlaying;
    
    public Recorder recorder;

    [SerializeField]
    private VoiceNetworkObject voiceNetworkObject;
    private bool _micOn;
    private bool _micStatus;
    
    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            this.enabled = false;
            return;
        }
           
        
        recorder = RunnerManager.Instance.runner.GetComponentInChildren<Recorder>();
        recorder.TransmitEnabled = false;
        
        PlayerInput.MicMute += ToggleMic;
    }

    private void Update()
    {
        if (!HasStateAuthority)
            return;
        
        _micStatus = (voiceNetworkObject.SpeakerInUse && voiceNetworkObject.IsSpeaking) || 
                     (voiceNetworkObject.RecorderInUse && voiceNetworkObject.IsRecording);

        //Debug.Log("마이크 입력체크 : " + _micStatus);
        IsPlaying?.Invoke(_micStatus);
    }

    private void ToggleMic(bool state)
    {
        if (HasStateAuthority)
        {
            recorder.TransmitEnabled = state;
            //Debug.Log("마이크 :"+ state);
        }
    }
}
