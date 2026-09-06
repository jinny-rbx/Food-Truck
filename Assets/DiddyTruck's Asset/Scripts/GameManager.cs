using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class MaterialRequirement
    {
        public string materialID = "Crystal";
        public int requiredAmount = 5;
        [HideInInspector] public int currentAmount = 0;
    }

    [Header("Requirements")]
    [SerializeField] private MaterialRequirement[] requiredMaterials;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Extraction Settings")]
    [SerializeField] private GameObject extractionZoneObject; // Drag your Exit Zone GameObject here

    [Header("References")]
    [SerializeField] private Energy playerEnergy; // Your energy script
    [SerializeField] private CollectionMeter collectionMeter;

    [Header("Star Requirements")]
    [SerializeField] private float targetTimeLimit = 120f; // Must complete under 60 seconds
    [SerializeField] private float maxEnergyLimit = 100f; // What counts as "Full" Energy
    [SerializeField] private int maxOrbsLimit = 20;       // Full meter target

    [Header("UI Star Visuals (Images or GameObjects)")]
    [SerializeField] private GameObject star1Icon;
    [SerializeField] private GameObject star2Icon;
    [SerializeField] private GameObject star3Icon;


    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject EndScreen;
    [SerializeField] private Health hp;
    public bool AreObjectivesMet { get; private set; }

    // Events
    public event Action OnObjectivesCompleted;
    public event Action OnLevelFinished;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UI.SetActive(true);
        EndScreen.SetActive(false);
        WinScreen.SetActive(false);
        Time.timeScale = 1f;

        // Disable extraction zone until objectives are met
        if (extractionZoneObject != null)
        {
            extractionZoneObject.SetActive(false);
        }

        UpdateObjectiveUI();
    }

    private void Update()
    {
        if (hp.health <= 0 || playerEnergy.CurrentEnergy <= 0)
        {
            End();
        }
    }
    public void CollectMaterial(string materialID, int amount)
    {
        if (AreObjectivesMet) return;

        bool itemFound = false;

        foreach (var req in requiredMaterials)
        {
            if (req.materialID.Equals(materialID, StringComparison.OrdinalIgnoreCase))
            {
                req.currentAmount = Mathf.Min(req.currentAmount + amount, req.requiredAmount);
                itemFound = true;
                break;
            }
        }

        if (itemFound)
        {
            UpdateObjectiveUI();
            CheckObjectives();
        }
    }

    private void CheckObjectives()
    {
        foreach (var req in requiredMaterials)
        {
            if (req.currentAmount < req.requiredAmount) return; // Still missing items
        }

        // All materials collected!
        AreObjectivesMet = true;

        if (extractionZoneObject != null)
        {
            extractionZoneObject.SetActive(true); // Activate the exit zone
        }

        UpdateObjectiveUI();
        OnObjectivesCompleted?.Invoke();
        Debug.Log("All materials gathered! Head to the extraction point!");
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveText == null) return;

        string display = "";

        foreach (var req in requiredMaterials)
        {
            string statusColor = req.currentAmount >= req.requiredAmount ? "<color=green>" : "<color=white>";
            display += $"{statusColor}- Collect {req.materialID}: {req.currentAmount}/{req.requiredAmount}</color>\n";
        }

        if (AreObjectivesMet)
        {
            display += "\n<color=yellow><b>>> HEAD TO EXTREACTION ZONE <<</b></color>";
        }

        objectiveText.text = display;
    }

    /// <summary>
    /// Called by ExtractionZone script when the player enters the area.
    /// </summary>
    public void ReachExtractionZone()
    {
        if (!AreObjectivesMet) return;

        Debug.Log("Challenge Completed! Player reached the extraction point.");
        OnLevelFinished?.Invoke();
        EvaluateLevelCompletion();
        Win();
    }

    public void EvaluateLevelCompletion()
    {
        int totalStars = 0;

        // Condition 1: Full Energy
        if (playerEnergy != null && playerEnergy.CurrentEnergy >= maxEnergyLimit)
        {
            totalStars++;
            Debug.Log("Star 1 Earned: Full Energy!");
        }

        // Condition 2: Full Collection Meter
        if (collectionMeter != null && collectionMeter.CurrentOrbs >= maxOrbsLimit)
        {
            totalStars++;
            Debug.Log("Star 2 Earned: Meter Full!");
        }

        totalStars++;
        Debug.Log("Star 3 Earned: Touched Extraction Zone!");

        // Display results
        DisplayStarsUI(totalStars);
    }

    private void DisplayStarsUI(int starCount)
    {
        // Enable icons based on total earned stars
        if (star1Icon != null) star1Icon.SetActive(starCount >= 1);
        if (star2Icon != null) star2Icon.SetActive(starCount >= 2);
        if (star3Icon != null) star3Icon.SetActive(starCount >= 3);
    }

    public void End()
    {
        UI.SetActive(false);
        EndScreen.SetActive(true);
        WinScreen.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Win()
    {
        UI.SetActive(false);
        EndScreen.SetActive(false);
        WinScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}