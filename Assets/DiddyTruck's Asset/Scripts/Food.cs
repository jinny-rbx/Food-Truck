using UnityEngine;

public class Food : MonoBehaviour
{
    public float points;
    public float energy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            player.EnergyManager(energy);
            player.SatisfactionManager(points);
            Destroy(gameObject);
        }
    }
}