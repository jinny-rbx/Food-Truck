using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] spawnPrefabs; // Array of items/orbs to spawn
    [SerializeField] private int spawnAmount = 10;       // How many total items to spawn

    [Header("Spawn Area Boundaries")]
    [SerializeField] private Vector3 centerOffset = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f); // Width, Height, Depth

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
        if (spawnPrefabs == null || spawnPrefabs.Length == 0) return;

        // 1. Pick a random prefab from the array
        GameObject selectedPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

        // 2. Generate a random point within the defined area box
        Vector3 origin = transform.position + centerOffset;
        Vector3 randomPoint = origin + new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );

        // 3. Optional: Snap object to Terrain/Ground using Raycast
        if (alignToGround)
        {
            Vector3 rayOrigin = new Vector3(randomPoint.x, origin.y + raycastStartHeight, randomPoint.z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastStartHeight * 2f, groundLayer))
            {
                randomPoint.y = hit.point.y; // Snap Y position directly onto terrain surface
            }
        }

        // 4. Instantiate the object with random rotation
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Instantiate(selectedPrefab, randomPoint, randomRotation, transform);
    }

    // Visualize the spawn zone in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position + centerOffset, spawnAreaSize);
    }
}