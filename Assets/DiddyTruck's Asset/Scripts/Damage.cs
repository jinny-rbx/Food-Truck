using UnityEngine;

public class Damage : MonoBehaviour
{
    public float health = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerControl>(out PlayerControl player))
            {
                player.HealthManager(-health);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Object tagged 'Player' is missing the PlayerControl component!");
            }
        }
    }
}