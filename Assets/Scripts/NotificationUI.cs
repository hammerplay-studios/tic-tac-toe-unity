using UnityEngine;
using TMPro;

public class NotificationUI : MonoBehaviour
{
    private TextMeshProUGUI notifyText;

    private void Awake()
    {
        notifyText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowText (string text)
    {
        gameObject.SetActive(true);
        notifyText.text = text;
    }

    public void Hide ()
    {
        gameObject.SetActive(false);
    }
}
