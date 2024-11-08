using UnityEngine;

public class TestPlayerJumpMove : MonoBehaviour
{
    private CharacterController cc;
    private Animator animator;

    public float moveSpeed = 5f;
    public float rotSpeed = 200f;

    private float mx = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerRotate();
    }

    private void PlayerMove()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0f, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);
        
        cc.Move(moveSpeed * Time.deltaTime * dir);
        
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
        mx += mouseX * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, mx, 0f);
    }
}
