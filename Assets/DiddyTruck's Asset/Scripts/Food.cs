using UnityEngine;

public class Food : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching it has the "Player" tag
        if (other.CompareTag("Player"))
        {
            print("touched");
            Energy playerEnergy = other.GetComponent<Energy>();

            playerEnergy.Heal(20);
            // Option 1: Completely destroy the object
            Destroy(gameObject);
        }
    }
}