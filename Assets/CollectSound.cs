using UnityEngine;

public class CollectSound : MonoBehaviour
{
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private float volume = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger hit by: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            if (coinSound != null)
            {
                AudioSource.PlayClipAtPoint(coinSound, transform.position, volume);
            }
            else
            {
                Debug.LogError("No Audio Clip assigned in the Inspector!");
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Object tag is '" + other.tag + "', but expected 'Player'");
        }
    }
}