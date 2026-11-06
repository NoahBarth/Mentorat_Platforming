using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float launchForce = 18f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("JumpPad Triggered by " + other.name);
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            return;
        }

        player.LaunchUpwards(launchForce);
    }
}   