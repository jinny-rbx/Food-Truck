using UnityEngine;
using UnityEngine.UI;

public class CollectionMeter : MonoBehaviour
{
    public static CollectionMeter Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Slider meterSlider;

    [Header("Settings")]
    [SerializeField] private int maxOrbs = 10;
    
    private int currentOrbs = 0;
    public int CurrentOrbs => currentOrbs;

    private void Awake()
    {
        // Singleton pattern for easy global access
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize UI bar limits
        if (meterSlider != null)
        {
            meterSlider.minValue = 0;
            meterSlider.maxValue = maxOrbs;
            meterSlider.value = 0;
        }
    }

    public void AddOrb(int amount = 1)
    {
        currentOrbs += amount;
        currentOrbs = Mathf.Clamp(currentOrbs, 0, maxOrbs);

        // Update Slider UI
        if (meterSlider != null)
        {
            meterSlider.value = currentOrbs;
        }

        // Check completion condition
        if (currentOrbs >= maxOrbs)
        {
            OnMeterFilled();
        }
    }

    private void OnMeterFilled()
    {
        Debug.Log("Collection Meter Full! Triggering Objective/Extraction...");
        // Add your event trigger here (e.g., ExtractionZone.Instance.Unlock();)
    }
}