using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotButton : MonoBehaviour
{

    private TextMeshProUGUI buttonText;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(ButtonClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void ButtonClick ()
    {
        // Send the message back to server.
        GameManager.Instance.AssignSlot(SlotIndex);
    }

    public int SlotIndex { get; private set; }

    public void SetIndex (int slotIndex)
    {
        this.SlotIndex = slotIndex;
    }

    public void SetText (string text)
    {
        buttonText.text = text;
    }
}
