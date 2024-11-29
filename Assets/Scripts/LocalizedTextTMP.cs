using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTextTMP : MonoBehaviour
{
    public string key;

    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("LocalizationManager is not initialized.");
            return;
        }

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Key is missing in LocalizedTextTMP.");
            return;
        }

        if (textComponent == null)
        {
            Debug.LogError("TextMeshProUGUI component is missing.");
            return;
        }

        string localizedText = LocalizationManager.Instance.GetText(key);
        textComponent.text = localizedText;
    }

}
