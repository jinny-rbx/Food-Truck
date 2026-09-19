using UnityEngine;
using UnityEngine.UIElements;

public class GameUIHandler : MonoBehaviour
{
    public PlayerControl PlayerControl;
    public GameManager GameManager;
    public UIDocument UIDoc;

    private Label m_HealthLabel;
    private VisualElement m_HealthBarMask;
    private VisualElement m_SatisfyBarMask;
    private VisualElement m_EnergyBarMask;

    private void Start()
    {
        // Auto-assign missing inspector references to prevent NullReferenceExceptions
        if (PlayerControl == null) PlayerControl = FindAnyObjectByType<PlayerControl>();
        if (GameManager == null) GameManager = GameManager.Instance ?? FindAnyObjectByType<GameManager>();
        if (UIDoc == null) UIDoc = GetComponent<UIDocument>();

        if (PlayerControl != null)
        {
            PlayerControl.OnHealthChange += HealthChanged;
            PlayerControl.OnSatisfyChange += SatisfactionChanged;
            PlayerControl.OnEnergyChange += EnergyChanged;
        }

        if (UIDoc != null && UIDoc.rootVisualElement != null)
        {
            VisualElement root = UIDoc.rootVisualElement;
            m_HealthLabel = root.Q<Label>("HealthLabel");
            m_HealthBarMask = root.Q<VisualElement>("HealthBarMask");
            m_SatisfyBarMask = root.Q<VisualElement>("SatisfyBarMask");
            m_EnergyBarMask = root.Q<VisualElement>("EnergyBarMask");
        }

        HealthChanged();
        SatisfactionChanged();
        EnergyChanged();
    }

    private void OnDisable()
    {
        if (PlayerControl != null)
        {
            PlayerControl.OnHealthChange -= HealthChanged;
            PlayerControl.OnSatisfyChange -= SatisfactionChanged;
            PlayerControl.OnEnergyChange -= EnergyChanged;
        }
    }

    void HealthChanged()
    {
        // Guard clause prevents calling methods on null references
        if (PlayerControl == null || GameManager == null) return;

        float healthRatio = (float)PlayerControl.currentHealth / GameManager.maxHealth;
        float healthPercent = Mathf.Lerp(8, 88, healthRatio);

        if (m_HealthBarMask != null)
        {
            m_HealthBarMask.style.width = Length.Percent(healthPercent);
        }

        if (m_HealthLabel != null)
        {
            m_HealthLabel.text = $"{PlayerControl.currentHealth}/{GameManager.maxHealth}";
        }
    }

    void SatisfactionChanged()
    {
        if (PlayerControl == null || GameManager == null || m_SatisfyBarMask == null) return;

        float satisfyRatio = (float)PlayerControl.currentSatisfy / GameManager.maxSatisfy;
        float satisfyPercent = Mathf.Lerp(35, 100, satisfyRatio);
        m_SatisfyBarMask.style.width = Length.Percent(satisfyPercent);
    }

    void EnergyChanged()
    {
        if (PlayerControl == null || GameManager == null || m_EnergyBarMask == null) return;

        float energyRatio = (float)PlayerControl.currentEnergy / GameManager.maxEnergy;
        float energyPercent = Mathf.Lerp(35, 100, energyRatio);
        m_EnergyBarMask.style.width = Length.Percent(energyPercent);
    }
}