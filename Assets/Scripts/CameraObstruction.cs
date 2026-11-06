using System.Collections.Generic;
using UnityEngine;

public class CameraObstruction : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 1.2f;

    private HashSet<DitherFadeObject> previousObstructions = new();

    void LateUpdate()
    {
        HashSet<DitherFadeObject> currentObstructions = new();

        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            radius,
            dir.normalized,
            distance,
            obstructionMask);

        bool IsPlayerStandingOn(DitherFadeObject fadeObject)
        {
            if (Physics.Raycast(player.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
            {
                DitherFadeObject groundFade = hit.collider.GetComponentInParent<DitherFadeObject>();
                return groundFade == fadeObject;
            }

            return false;
        }
        foreach (var hit in hits)
        {
            Debug.Log("Hit: " + hit.collider.name);

            DitherFadeObject fade =
                hit.collider.GetComponentInParent<DitherFadeObject>();

            if (fade == null)
            {
                Debug.Log("No DitherFadeObject on " + hit.collider.name);
                continue;
            }

            if (IsPlayerStandingOn(fade))
                continue;

            fade.SetFade(0.35f);
            currentObstructions.Add(fade);
        }


        foreach (var fade in previousObstructions)
        {
            if (!currentObstructions.Contains(fade))
                fade.SetFade(1f);
        }

        previousObstructions = currentObstructions;
    }
}