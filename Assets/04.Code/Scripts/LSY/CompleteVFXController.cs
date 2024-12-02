using System.Collections;
using UnityEngine;

public class CompleteVFXController : MonoBehaviour
{
    public ParticleSystem bigFireWorkEffect;
    public ParticleSystem smallFireWorkEffect1;
    public ParticleSystem smallFireWorkEffect2;
    public ParticleSystem smallFireWorkEffect3;
    public AudioSource audioSource;

    public void DoPlayFireWorkEffect()
    {
        StartCoroutine(PlayFireWorkEffect());
    }
    public IEnumerator PlayFireWorkEffect()
    {
        audioSource.Play();
        bigFireWorkEffect.Play();
        yield return new WaitForSeconds(3f);
        smallFireWorkEffect1.Play();
        yield return new WaitForSeconds(1f);
        smallFireWorkEffect2.Play();
        yield return new WaitForSeconds(1f);
        smallFireWorkEffect3.Play();
        yield return null;

    }
}
