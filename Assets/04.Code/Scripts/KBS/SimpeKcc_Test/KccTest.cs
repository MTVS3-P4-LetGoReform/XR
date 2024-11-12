using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KccTest : NetworkBehaviour
{
    private Animator animator;
    private NetworkMecanimAnimator NetworkMecanimAnimator;

    private NetworkCharacterController networkCC;

    private Camera camera;

    [SerializeField]
    private float moveSpeed = 20f;
    [SerializeField]
    private float rotSpeed = 200f;

    private float mx = 0f;

    private bool _onChat = false;
    
    private void Awake()
    {
        networkCC = GetComponent<NetworkCharacterController>();
        animator = GetComponentInChildren<Animator>();
        NetworkMecanimAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        }

        PlayerInput.OnChat += StopMoving;
        PlayerInput.OnMessenger += StopMoving;
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            var readyCheck = FindAnyObjectByType<ReadyCheck>();
            readyCheck.gameStartButton.gameObject.SetActive(true);

            GameStateManager.Instance.Complete += StopMoving;
        }
    }

    private void StopMoving(bool onChat)
    {
        _onChat = onChat;
    }


    public override void FixedUpdateNetwork()
    {
        if(!HasStateAuthority) return;

        if (_onChat)
            return;
        
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
