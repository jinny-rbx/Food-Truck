using System;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnGroup
    {
        public string groupName;
        public GameObject[] variations;
    }

    [Header("Spawn Settings")]
    [SerializeField] private SpawnGroup[] spawnGroups;
    [SerializeField] private int spawnAmount = 10;

    [Header("Spawn Area Boundaries")]
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(50f, 0f, 50f);

    [Header("Ground Alignment")]
    [SerializeField] private bool alignToGround = true;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float surfaceOffset = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

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

        // 1. Pick a random group & prefab
        SpawnGroup selectedGroup = spawnGroups[UnityEngine.Random.Range(0, spawnGroups.Length)];
        if (selectedGroup.variations == null || selectedGroup.variations.Length == 0) return;

        GameObject selectedPrefab = selectedGroup.variations[UnityEngine.Random.Range(0, selectedGroup.variations.Length)];
        if (selectedPrefab == null) return;

        // 2. Random XZ point relative to Spawner position
        Vector3 origin = transform.position + centerOffset;
        float randomX = UnityEngine.Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomZ = UnityEngine.Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);

        Vector3 spawnPosition = origin + new Vector3(randomX, 0f, randomZ);

        if (alignToGround)
        {
            // Raycast starting 500 units ABOVE the spawner, casting 1000 units DOWN
            Vector3 rayOrigin = new Vector3(spawnPosition.x, transform.position.y + 500f, spawnPosition.z);
            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, 1000f, groundLayer, QueryTriggerInteraction.Ignore);

            bool foundGround = false;
            float targetY = 0f;

            foreach (var hit in hits)
            {
                // Detect TerrainCollider OR any mesh/collider on the Terrain layer
                if (hit.collider is TerrainCollider || hit.collider.GetComponent<Terrain>() != null || ((1 << hit.collider.gameObject.layer) & groundLayer) != 0)
                {
                    // Ensure we ignore steep foliage/tree colliders (ground normal points upward)
                    if (hit.normal.y > 0.3f)
                    {
                        targetY = hit.point.y;
                        foundGround = true;

                        if (showDebugRays)
                        {
                            Debug.DrawLine(rayOrigin, hit.point, Color.green, 10f);
                        }
                        break;
                    }
                }
            }

            // CRITICAL: If raycast completely missed the ground, DO NOT SPAWN!
            if (!foundGround)
            {
                if (showDebugRays)
                {
                    Debug.DrawRay(rayOrigin, Vector3.down * 1000f, Color.red, 10f);
                }
                return; // Stop execution here so items never float in mid-air
            }

            spawnPosition.y = targetY + surfaceOffset;
        }

        // 3. Instantiate object onto valid ground point
        Quaternion randomRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        Instantiate(selectedPrefab, spawnPosition, randomRotation, transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position + centerOffset, spawnAreaSize);
    }
}