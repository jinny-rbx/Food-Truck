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

    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject EndScreen;
    [SerializeField] private Timer timer;
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
        if (hp.health <= 0 || timer.RemainingTime <= 0)
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
        // Add win screen or level transition logic here
        Win();
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