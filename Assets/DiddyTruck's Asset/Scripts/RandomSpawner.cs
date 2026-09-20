using System;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnGroup
    {
        public string groupName; // e.g., "Good Food", "Bad Food", "Damage"
        public GameObject[] variations; // List of models/prefabs for this type
    }

    [Header("Spawn Settings")]
    [SerializeField] private SpawnGroup[] spawnGroups;
    [SerializeField] private int spawnAmount = 10;

    [Header("Spawn Area Boundaries")]
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);

    [Header("Ground Alignment (3D Map)")]
    [SerializeField] private bool alignToGround = true;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float surfaceOffset = 0.5f; // Adjust if items clip slightly into mesh

    [Header("Debug Visualizer")]
    [SerializeField] private bool showDebugRays = true;
    [SerializeField] private float debugRayDuration = 10f; // Seconds ray lines stay visible in Scene View

    private void Start()
    {
        SpawnAllObjects();
    }

    public void SpawnAllObjects()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SpawnSingleObject();
        }
    }

    private void SpawnSingleObject()
    {
        if (spawnGroups == null || spawnGroups.Length == 0) return;

        // 1. Pick a random spawn group
        SpawnGroup selectedGroup = spawnGroups[UnityEngine.Random.Range(0, spawnGroups.Length)];
        if (selectedGroup.variations == null || selectedGroup.variations.Length == 0) return;

        // 2. Pick a random prefab variation
        GameObject selectedPrefab = selectedGroup.variations[UnityEngine.Random.Range(0, selectedGroup.variations.Length)];
        if (selectedPrefab == null) return;

        // 3. Generate random XZ coordinates within the spawn box bounds
        Vector3 origin = transform.position + centerOffset;
        float randomX = UnityEngine.Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomZ = UnityEngine.Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);

        Vector3 spawnPosition = origin + new Vector3(randomX, 0f, randomZ);

        // 4. Snap object strictly to top of Terrain/Ground surface
        if (alignToGround)
        {
            // Start ray high above the sky to ensure raycast fires from ABOVE the highest terrain peak
            float skyHeight = 1000f;
            Vector3 rayOrigin = new Vector3(spawnPosition.x, skyHeight, spawnPosition.z);

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, skyHeight * 2f, groundLayer, QueryTriggerInteraction.Ignore))
            {
                // Only snap if hitting a top-facing surface (hit normal pointing upwards)
                if (hit.normal.y > 0.3f)
                {
                    spawnPosition.y = hit.point.y + surfaceOffset;

                    if (showDebugRays)
                    {
                        Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, Color.green, debugRayDuration);
                        Debug.DrawLine(hit.point, hit.point + Vector3.up * 2f, Color.cyan, debugRayDuration);
                    }
                }
                else
                {
                    // Hit an underside face or vertical cliff - skip spawning
                    return;
                }
            }
            else
            {
                // Raycast missed the terrain layer - draw red ray and skip
                if (showDebugRays)
                {
                    Debug.DrawRay(rayOrigin, Vector3.down * (skyHeight * 2f), Color.red, debugRayDuration);
                }
                return;
            }
        }

        // 5. Instantiate object with random Y rotation
        Quaternion randomRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        Instantiate(selectedPrefab, spawnPosition, randomRotation, transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position + centerOffset, spawnAreaSize);
    }
}