using UnityEngine;

public class Coollectible : MonoBehaviour
{
    [SerializeField] private string materialID = "Crystal"; // Identifies the item type
    [SerializeField] private int amount = 1;
    [SerializeField] private AudioClip collectSound;
    public float points;
    public GameObject obj;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            player.SatisfactionManager(points);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectMaterial(materialID, amount);
                if (SoundManager.Instance != null && collectSound != null)
                {
                    SoundManager.Instance.PlaySound2D(collectSound);
                }
                Destroy(obj);
            }
        }
    }
}