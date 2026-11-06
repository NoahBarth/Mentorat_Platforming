using UnityEngine;

public class AnimateCube : MonoBehaviour, IMovingPlatform
{
    public float moveDistance = 10f;
    public float speed = 1f;

    public enum Direction { X, Y, Z }
    [SerializeField] private Direction direction = Direction.X;

    public enum MoveDirection { Positive, Negative }
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Positive;

    private Vector3 startPosition;
    private Vector3 lastPosition;

    public Vector3 DeltaPosition { get; private set; }
    public Quaternion DeltaRotation => Quaternion.identity;
    public Vector3 Pivot => transform.position;

    void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, moveDistance);

        if (moveDirection == MoveDirection.Negative)
        {
            offset = -offset;
        }

        Vector3 position = startPosition;

        switch (direction)
        {
            case Direction.X: position.x += offset; break;
            case Direction.Y: position.y += offset; break;
            case Direction.Z: position.z += offset; break;
        }

        transform.position = position;

        DeltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;
    }
}