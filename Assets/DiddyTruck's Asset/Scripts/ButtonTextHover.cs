using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // Use UnityEngine.UI if you are not using TextMeshPro

public class ButtonTextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text Component")]
    public TMP_Text buttonText; // Change to 'Text' if using legacy UI

    [Header("Font Sizes")]
    public float normalSize = 24f;
    public float hoverSize = 30f;

    void Awake()
    {
        // Auto-get text if not assigned manually
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
            buttonText.fontSize = normalSize;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.fontSize = hoverSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.fontSize = normalSize;
    }
}