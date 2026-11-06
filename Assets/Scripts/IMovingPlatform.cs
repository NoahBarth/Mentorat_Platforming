using UnityEngine;

public interface IMovingPlatform
{
    Vector3 DeltaPosition { get; }
    Quaternion DeltaRotation { get; }
    Vector3 Pivot { get; }
}