using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private string hoverSoundName = "ButtonHover";
    [SerializeField] private string clickSoundName = "ButtonPressed";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(hoverSoundName))
        {
            SoundManager.Instance.PlaySound2D(hoverSoundName);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(clickSoundName))
        {
            SoundManager.Instance.PlaySound2D(clickSoundName);
        }
    }
}