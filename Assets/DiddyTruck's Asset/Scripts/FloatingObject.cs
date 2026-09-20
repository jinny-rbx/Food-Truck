using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatSpeed = 2f;    // How fast it moves up and down
    [SerializeField] private float floatHeight = 0.5f; // How high and low it floats

    [Header("Rotation Settings (Optional)")]
    [SerializeField] private bool rotate = true;
    [SerializeField] private float rotateSpeed = 50f;  // Speed of rotation

    private Vector3 startPosition;

    void Start()
    {
        // Save the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate new Y position using a Sine wave
        float newY = startPosition.y + (Mathf.Sin(Time.time * floatSpeed) * floatHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Optional: Slowly rotate the object
        if (rotate)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}