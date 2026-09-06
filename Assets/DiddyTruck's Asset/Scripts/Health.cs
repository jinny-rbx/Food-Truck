using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class Health:MonoBehaviour
{

    public Image healthBar;

    public float health, maxHealth = 100;
    float lerpSpeed;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {

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
