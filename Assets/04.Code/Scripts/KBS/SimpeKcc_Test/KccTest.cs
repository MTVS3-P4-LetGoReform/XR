using Fusion;
using UnityEngine;

public class KccTest : NetworkBehaviour
{
    private Animator animator;

    private NetworkCharacterController networkCC;

    private Camera camera;

    [SerializeField]
    private float moveSpeed = 20f;
    [SerializeField]
    private float rotSpeed = 200f;

    private float mx = 0f;


    private void Awake()
    {
        networkCC = GetComponent<NetworkCharacterController>();
        animator = GetComponentInChildren<Animator>();
        camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            
        }
    } 


    public override void FixedUpdateNetwork()
    {
        if(!HasStateAuthority) return;
        
        PlayerMove();
        PlayerRotate();
        PlayerJump();
    }

    private void PlayerMove()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        Vector3 dir = new Vector3(h, 0f, v);
        dir.Normalize();
        dir = camera.transform.TransformDirection(dir);
        dir.y = 0;
        
        networkCC.Move(moveSpeed * Runner.DeltaTime * dir);
        
        
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            animator.SetBool("IsWalking", false);
        }
        else
        {
            animator.SetBool("IsWalking", true);
        } 
    } 

    private void PlayerRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        mx += mouseX * networkCC.rotationSpeed * Runner.DeltaTime;
        transform.eulerAngles = new Vector3(0f, mx, 0f);
    }

    private void PlayerJump()
    {
        if (Input.GetButtonDown("Jump") && networkCC.Grounded)
        {
            networkCC.Velocity += Vector3.up * networkCC.jumpImpulse;
        }
    }
}
