using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageSelectorTMP : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private LocalizationManager localizationManager;

    void Start()
    {
        // Ensure the LocalizationManager instance exists
        localizationManager = LocalizationManager.Instance;

        if (localizationManager == null)
        {
            Debug.LogError("LocalizationManager instance is missing!");
            return;
        }

        if (localizationManager.languages == null || localizationManager.languages.Count == 0)
        {
            Debug.LogError("No languages found in LocalizationManager!");
            return;
        }

        // Populate dropdown with available languages
        dropdown.ClearOptions();
        var options = new List<string>();

        foreach (var language in localizationManager.languages)
        {
            Debug.Log($"Adding language: {language.languageName}");
            options.Add(language.languageName);
        }

        dropdown.AddOptions(options);

        // Add listener to detect dropdown value changes
        dropdown.onValueChanged.AddListener(OnLanguageSelected);

        // Set default language
        SetDefaultLanguage();
    }

    void OnLanguageSelected(int index)
    {
        if (localizationManager == null)
        {
            Debug.LogError("LocalizationManager is not assigned or initialized.");
            return;
        }

        if (index < 0 || index >= localizationManager.languages.Count)
        {
            Debug.LogError("Selected index is out of range.");
            return;
        }

        string selectedLanguage = localizationManager.languages[index].languageName;
        if (string.IsNullOrEmpty(selectedLanguage))
        {
            Debug.LogError("Selected language name is empty or null.");
            return;
        }

        Debug.Log($"Language selected: {selectedLanguage}");
        localizationManager.SetLanguage(selectedLanguage);

        // Update all localized TMP text elements
        foreach (var localizedText in FindObjectsOfType<LocalizedTextTMP>())
        {
            localizedText.UpdateText();
        }
    }

    void SetDefaultLanguage()
    {
        if (dropdown == null)
        {
            Debug.LogError("Dropdown is not assigned.");
            return;
        }

        if (localizationManager == null || localizationManager.languages.Count == 0)
        {
            Debug.LogError("LocalizationManager has no languages assigned.");
            return;
        }

        dropdown.value = 0; // Default to the first language
        OnLanguageSelected(0); // Trigger language update
    }
}
