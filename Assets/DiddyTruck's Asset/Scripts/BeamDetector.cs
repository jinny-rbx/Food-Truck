using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamDetector : MonoBehaviour
{
    [Header("Beam Settings")]
    [SerializeField] private float maxBeamDistance = 10f;
    [SerializeField] private Transform beamOrigin; // Assign an empty GameObject placed at the enemy's eyes/front
    [SerializeField] private LayerMask detectionMask; // Set this to check Player and Obstacles

    [Header("Detection Event")]
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color detectedColor = Color.red;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (beamOrigin == null) beamOrigin = transform;

        // Visual setup
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = normalColor;
        lineRenderer.endColor = normalColor;
    }

    void Update()
    {
        Vector3 startPos = beamOrigin.position;
        Vector3 forwardDir = beamOrigin.forward;

        // Perform Raycast
        if (Physics.Raycast(startPos, forwardDir, out RaycastHit hit, maxBeamDistance, detectionMask))
        {
            // Set end point of the laser to where it hit
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, hit.point);

            // Check if what we hit is the player
            if (hit.collider.CompareTag("Player"))
            {
                OnPlayerDetected(hit.collider.gameObject);
            }
            else
            {
                OnPlayerLost();
            }
        }
        else
        {
            // If nothing was hit, extend beam to max distance
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, startPos + forwardDir * maxBeamDistance);
            OnPlayerLost();
        }
    }

    private void OnPlayerDetected(GameObject player)
    {
        lineRenderer.startColor = detectedColor;
        lineRenderer.endColor = detectedColor;
        Debug.Log("Player Detected by Beam!");
        
        // Add your alert / trigger logic here (e.g., notify GameManager or attack)
    }

    private void OnPlayerLost()
    {
        lineRenderer.startColor = normalColor;
        lineRenderer.endColor = normalColor;
    }
}