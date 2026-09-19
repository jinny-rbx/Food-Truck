using UnityEngine;

public class Coollectible : MonoBehaviour
{
    [SerializeField] private string materialID = "Crystal"; // Identifies the item type
    [SerializeField] private int amount = 1;
    public float points;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            player.SatisfactionManager(points);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectMaterial(materialID, amount);
                Destroy(gameObject);
            }
        }
    }
}