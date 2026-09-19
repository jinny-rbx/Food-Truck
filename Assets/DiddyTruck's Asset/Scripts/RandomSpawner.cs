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
    [SerializeField] private SpawnGroup[] spawnGroups; // Replaces spawnPrefabs
    [SerializeField] private int spawnAmount = 10;

    [Header("Spawn Area Boundaries")]
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);

    [Header("Ground Alignment (3D Map)")]
    [SerializeField] private bool alignToGround = true;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastStartHeight = 50f;

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

        // 1. Randomly pick one of the spawn groups (e.g., badFood, Collect, Damage, goodFood)
        SpawnGroup selectedGroup = spawnGroups[UnityEngine.Random.Range(0, spawnGroups.Length)];

        // Safety check: skip if the selected group has no variations assigned
        if (selectedGroup.variations == null || selectedGroup.variations.Length == 0) return;

        // 2. Randomly pick one prefab variation from within that group
        GameObject selectedPrefab = selectedGroup.variations[UnityEngine.Random.Range(0, selectedGroup.variations.Length)];

        if (selectedPrefab == null) return;

        // 3. Generate a random point within the defined area box
        Vector3 origin = transform.position + centerOffset;
        Vector3 randomPoint = origin + new Vector3(
            UnityEngine.Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            UnityEngine.Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            UnityEngine.Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );

        // 4. Snap object to Terrain/Ground using Raycast
        if (alignToGround)
        {
            Vector3 rayOrigin = new Vector3(randomPoint.x, origin.y + raycastStartHeight, randomPoint.z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastStartHeight * 2f, groundLayer))
            {
                randomPoint.y = hit.point.y;
            }
        }

        // 5. Instantiate with random rotation
        Quaternion randomRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        Instantiate(selectedPrefab, randomPoint, randomRotation, transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position + centerOffset, spawnAreaSize);
    }
}