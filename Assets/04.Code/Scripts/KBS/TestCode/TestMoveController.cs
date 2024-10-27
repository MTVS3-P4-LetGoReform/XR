using UnityEngine;

public class TestMoveController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float moveSpeed = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Fly");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 dirForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 dirSide = Camera.main.transform.right;
        Vector3 dirUp = Vector3.up;

        Vector3 moveDir = (inputX * dirSide) + (inputY * dirUp) + (inputZ * dirForward);

        Camera.main.transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
