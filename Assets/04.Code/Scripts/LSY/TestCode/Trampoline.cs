using Fusion;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Trampoline : MonoBehaviour
{
    private NetworkCharacterController networkCC;
    private float jumpHeight = 40f;
    //public NetworkMecanimAnimator trampolineAnim;
    public Animator anim;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trampline other naem : " + other, other.gameObject);
            if (anim.GetBool("isBounce") == false)
            {
                anim.SetBool("isBounce", true);
                Debug.Log("Trampoline : OncollisionEnter");
                networkCC = other.gameObject.GetComponent<NetworkCharacterController>();
                networkCC.Velocity += Vector3.up * jumpHeight;
                anim.SetBool("isBounce", false);
            }
        }
    }
}
