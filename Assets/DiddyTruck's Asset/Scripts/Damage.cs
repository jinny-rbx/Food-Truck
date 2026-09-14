using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching it has the "Player" tag
        if (other.CompareTag("Player"))
        {
            print("touched");
            PlayerControl playerHP = other.GetComponent<PlayerControl>();
            playerHP.HealthManager(-damage);
            // Option 1: Completely destroy the object
            Destroy(gameObject);
        }
    }
}