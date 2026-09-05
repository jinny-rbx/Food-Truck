using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health:MonoBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject End;


    public TMP_Text healthText;
    public Image healthBar;

    float health, maxHealth = 100;
    float lerpSpeed;

    private void Start()
    {
        UI.SetActive(true);
        End.SetActive(false);
        Time.timeScale = 1f;
        health = maxHealth;
    }

    private void Update()
    {
        if (health <= 0)
        {
            UI.SetActive(false);
            End.SetActive(true);
            Time.timeScale = 0f;
        }

        healthText.text = health + "%";

        lerpSpeed = 3f * Time.deltaTime;

        HealthBarFiller();
        ColorChanger();
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health / maxHealth, lerpSpeed);
    }

    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));
        healthBar.color = healthColor;
    }

    public void Damage(float damagePoints)
    {
        if (health > 0)
            health -= damagePoints;
    }
}
