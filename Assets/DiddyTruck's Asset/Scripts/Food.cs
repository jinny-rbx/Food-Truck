using UnityEngine;

public class Food : MonoBehaviour
{
    public float points = 10f;
    public float energy = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // GetComponentInParent handles cases where the collider is on a child GameObject
            PlayerControl player = other.GetComponentInParent<PlayerControl>();

            if (player != null)
            {
                player.EnergyManager(energy);
                player.SatisfactionManager(points);

                // Play SFX if SoundManager exists
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySound2D("FoodPickup");
                }

                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[Food] Touched object tagged 'Player', but no PlayerControl component was found!");
            }
        }
    }
}