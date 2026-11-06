using UnityEngine;

public class RotatingPlatform : MonoBehaviour, IMovingPlatform
{
    [Tooltip("If true, rotates around the object's own local up axis (use this for a tilted log). If false, uses the fixed world-space axis below.")]
    public bool useLocalUpAxis = true;

    public Vector3 rotationAxis = Vector3.up; // only used if useLocalUpAxis is false
    public float degreesPerSecond = 30f;

    private Quaternion lastRotation;
    private Vector3 lastPosition;

    public Vector3 DeltaPosition { get; private set; }
    public Quaternion DeltaRotation { get; private set; }
    public Vector3 Pivot => transform.position;

    void Start()
    {
        lastRotation = transform.rotation;
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 axis = useLocalUpAxis ? transform.up : rotationAxis;

        transform.Rotate(axis, degreesPerSecond * Time.deltaTime, Space.World);

        DeltaPosition = transform.position - lastPosition;
        DeltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}