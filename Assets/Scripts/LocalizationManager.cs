using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public List<LanguageData> languages = new List<LanguageData>();
    public static LocalizationManager Instance { get; private set; }
    private Dictionary<string, string> currentLanguageDictionary = new Dictionary<string, string>();
    private string currentLanguage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(string languageName)
    {
        foreach (var language in languages)
        {
            if (language.languageName == languageName)
            {
                currentLanguageDictionary.Clear();
                foreach (var entry in language.entries)
                {
                    currentLanguageDictionary[entry.key] = entry.value;
                }
                currentLanguage = languageName;
                Debug.Log($"Language set to: {currentLanguage}");
                return;
            }
        }

        Debug.LogError($"Language {languageName} not found!");
    }

    public string GetText(string key)
    {
        if (currentLanguageDictionary.TryGetValue(key, out var value))
        {
            return value;
        }
        Debug.LogError($"Key {key} not found in language {currentLanguage}");
        return $"[Missing: {key}]";
    }
}
