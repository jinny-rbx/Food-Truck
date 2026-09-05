using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Energy : MonoBehaviour
{
    public Image energyBar;

    float energy, maxEnergy = 100;
    float lerpSpeed;

    private void Start()
    {
        energy = maxEnergy;
    }

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;

        EnergyBarFiller();
        ColorChanger();
    }

    void EnergyBarFiller()
    {
        energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount, energy / maxEnergy, lerpSpeed);
    }

    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.orange, (energy / maxEnergy));
        energyBar.color = healthColor;
    }

    public void Damage(float damagePoints)
    {
        if (energy > 0)
            energy -= damagePoints;
    }

    public void Heal(float damagePoints)
    {
        if (energy > 0)
            energy += damagePoints;
    }
}
