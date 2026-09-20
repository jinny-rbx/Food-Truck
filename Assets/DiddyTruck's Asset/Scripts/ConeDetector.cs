using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ConeDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Light spotlight; // Drag your Light component here (optional)

    public Action<PlayerControl> OnPlayerDetected;

    private float maxDistance = 10f;

    private void Start()
    {
        // Automatically fetch Light range if attached
        if (spotlight == null)
        {
            spotlight = GetComponentInChildren<Light>();
        }

        if (spotlight != null)
        {
            maxDistance = spotlight.range;
        }
    }

    // Runs every frame the player stays inside your BoxCollider
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerControl>(out PlayerControl player))
        {
            Vector3 origin = transform.position;
            Vector3 targetPos = other.transform.position;
            float distToPlayer = Vector3.Distance(origin, targetPos);

            // Check if player is within the Spotlight range
            if (distToPlayer <= maxDistance)
            {
                // Line of sight check to make sure walls aren't blocking
                if (!Physics.Linecast(origin, targetPos, obstacleMask))
                {
                    Debug.Log("<color=yellow>Player spotted inside BoxCollider detection zone!</color>");
                    OnPlayerDetected?.Invoke(player);
                }
            }
        }
    }
}