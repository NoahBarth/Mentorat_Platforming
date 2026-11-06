using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    
    public Transform player; // Assign your player
    public float turnSpeed = 1.0f;
    public float heightOffset = 2.0f;
    public float distance = 8.0f;
    private Vector3 offset;
    public float minPitch = -10f;
    public float maxPitch = 60f;
    private float yaw = 0f;
    private float pitch = 20f;

    void Start()
    {
        offset = new Vector3(0, heightOffset, -distance);
        // initialize yaw/pitch from current transform
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    public void RotateCamera(Vector2 rotationVector) {
        float horizontal = rotationVector.x * turnSpeed;
        float vertical = rotationVector.y * turnSpeed;
        // update yaw/pitch and clamp pitch
        yaw += horizontal;
        pitch += -vertical;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // compute direction from yaw/pitch
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rot * Vector3.forward;

        // set position and look at player
        transform.position = player.position - dir * distance + Vector3.up * heightOffset;
        transform.LookAt(player.position + Vector3.up * (heightOffset * 0.5f));
    }
}