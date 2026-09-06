using UnityEngine;

public class Coollectible : MonoBehaviour
{
    [SerializeField] private string materialID = "Crystal"; // Identifies the item type
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Adjust tag check based on your Player setup
        if (other.CompareTag("Player"))
        {
            if (CollectionMeter.Instance != null)
            {
                CollectionMeter.Instance.AddOrb(1);
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectMaterial(materialID, amount);
                Destroy(gameObject);
            }
        }
    }
}