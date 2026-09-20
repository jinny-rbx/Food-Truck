using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class DynamicMessageUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Message Pool")]
    [TextArea(2, 4)]
    [SerializeField] private string[] messageList;

    private void OnEnable()
    {
        // Automatically picks a random message every time this UI panel is activated
        DisplayRandomMessage();
    }

    public void DisplayRandomMessage()
    {
        if (messageText == null)
        {
            Debug.LogWarning("Message Text reference is missing on " + gameObject.name);
            return;
        }

        if (messageList != null && messageList.Length > 0)
        {
            int randomIndex = Random.Range(0, messageList.Length);
            messageText.text = messageList[randomIndex];
        }
    }
}