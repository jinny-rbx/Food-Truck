using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health:MonoBehaviour
{
    public TMP_Text healthText;
    public Image healthBar;

    float health, maxHealth = 100;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        healthText.text = "Health: " + health + "%";

        HealthBarFiller();

    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = health / maxHealth;
    }

    public void Damage(float damagePoints)
    {
        if (health > 0)
            health -= damagePoints;
    }
}
