using UnityEngine;

public class objrot : MonoBehaviour
{
    private float rotSpeed = 100.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0),Space.World);
    }
}
