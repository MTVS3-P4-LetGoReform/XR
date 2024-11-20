using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestPlayerMove : MonoBehaviour
{

   // public AudioSource audioWalking;
    
    
    private Animator animator;
    //private NetworkMecanimAnimator NetworkMecanimAnimator;
    //private NetworkCharacterController networkCC;

    private CharacterController cc;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private float moveSpeed = 20f;
    [SerializeField]
    private float rotSpeed = 200f;

    private float mx = 0f;
    
    private bool _jump;
    private bool _isStop = false;
    
    private const string PlayScene = "Alpha_PlayScene";
    
    private void Awake()
    {
        //networkCC = GetComponent<NetworkCharacterController>();
        animator = GetComponentInChildren<Animator>();
        //NetworkMecanimAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /* public override void Spawned()
    {
        if (HasStateAuthority)
        {
            camera = GetComponentInChildren<Camera>();
        }

        PlayerInput.OnChat += StopMoving;
        PlayerInput.OnMessenger += StopMoving;
        PlayerInput.OnMouse += StopMoving;
        
        if (SceneManager.GetActiveScene().name == PlayScene)
        {
            GameStateManager.Instance.Complete += StopMoving;
        }
    } */

    private void StopMoving(bool isActive)
    {
        _isStop = isActive;
    }


    private void Update()
    {
       /* if (Input.GetButtonDown("Jump") && cc.Grounded)
        {
            _jump = true;
            NetworkMecanimAnimator.SetTrigger("IsJumping");
        } */
       
       PlayerMove();
       PlayerRotate();
    }

   /* public override void FixedUpdateNetwork()
    {
        if(!HasStateAuthority) return;

        cc.Move(Vector3.zero);
        
        if (_isStop)
            return;
        
        PlayerMove();
        PlayerRotate();
        if (_jump)
        {
            _jump = false;
            PlayerJump();
        }
    } */

    private void PlayerMove()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        Vector3 dir = new Vector3(h, 0f, v);
        dir.Normalize();
        dir = camera.transform.TransformDirection(dir);
        dir.y = 0; 
        
        cc.Move(moveSpeed * Time.deltaTime * dir);
        
        
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            animator.SetBool("IsWalking", false);
           // audioWalking.Stop();
        }
        else
        {
            animator.SetBool("IsWalking", true);
            
          /*  if (!audioWalking.isPlaying)
            {
                audioWalking.Play();
                audioWalking.loop = true;
            } */
        }

        
    } 

    private void PlayerRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        mx += mouseX *  Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, mx, 0f);
    }

    private void PlayerJump()
    {
       // networkCC.Velocity += Vector3.up * networkCC.jumpImpulse;
    }
}
