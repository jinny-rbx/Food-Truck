using UnityEngine;

public class PlayerEvent : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching it has the "Player" tag
        if (other.CompareTag("Player"))
        {
            print("touched");
            Health playerHP = other.GetComponent<Health>();

            playerHP.Damage(50);
            // Option 1: Completely destroy the object
            Destroy(gameObject);
        }
    }
}