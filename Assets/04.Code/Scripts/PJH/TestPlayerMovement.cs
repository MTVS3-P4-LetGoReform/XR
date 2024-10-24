using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    private float _currentSpeed = 3f;
    private float _yVelocity = -10f;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        Move();
    }
    
    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, 0f, v).normalized;
        dir = transform.TransformDirection(dir) * _currentSpeed;
        dir.y = _yVelocity;
        _controller.Move(dir * Time.deltaTime);
    }
}
