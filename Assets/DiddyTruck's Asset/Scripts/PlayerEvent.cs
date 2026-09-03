using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching it has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Option 1: Completely destroy the object
            Destroy(gameObject);

            // Option 2: Hide it temporarily instead (Uncomment below if needed)
            // gameObject.SetActive(false);
        }
    }
}