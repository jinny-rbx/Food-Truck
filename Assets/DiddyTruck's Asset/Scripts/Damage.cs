using UnityEngine;

public class Damage : MonoBehaviour
{
    public float healthDamage = 10f;
    public GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerControl>(out PlayerControl player))
            {
                player.HealthManager(-healthDamage);
                Destroy(enemy);
            }
        }
    }
}