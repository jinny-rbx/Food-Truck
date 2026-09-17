using UnityEngine;

public class TargetWaypoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform waypointUI;
    [SerializeField] private Transform targetLocation;
    [SerializeField] private Camera mainCamera;

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float edgePadding = 50f; // Distance from screen borders in pixels

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (targetLocation == null || waypointUI == null) return;

        // 1. Convert world position to raw screen point
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetLocation.position + offset);

        // 2. Handle objects behind the camera
        if (screenPos.z < 0)
        {
            // Invert vector so off-screen target behind player clamps to the bottom/correct side
            screenPos *= -1f;
        }

        // 3. Define screen boundaries with padding
        float minX = edgePadding;
        float maxX = Screen.width - edgePadding;
        float minY = edgePadding;
        float maxY = Screen.height - edgePadding;

        // 4. Clamp screen positions to keep indicator on screen
        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        // 5. Apply position (keeping original Z for depth if needed)
        waypointUI.position = new Vector3(screenPos.x, screenPos.y, 0f);
    }
}