using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LocalizationTool : EditorWindow
{
    [SerializeField]
    private LanguageData masterLanguage;

    [SerializeField]
    private string targetFolder = "Assets/Languages";

    private List<LanguageData> allLanguages = new List<LanguageData>();
    private string[] languageNames;
    private int selectedLanguageIndex = 0;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Localization Tool")]
    public static void ShowWindow()
    {
        LocalizationTool window = GetWindow<LocalizationTool>("Localization Tool");
        window.minSize = new Vector2(800, 600);
        window.LoadLanguages();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Master Language", EditorStyles.boldLabel);
        masterLanguage = (LanguageData)EditorGUILayout.ObjectField("Master Language", masterLanguage, typeof(LanguageData), false);

        EditorGUILayout.LabelField("Target Folder", EditorStyles.boldLabel);
        targetFolder = EditorGUILayout.TextField("Folder Path", targetFolder);

        if (GUILayout.Button("Reload Languages"))
        {
            LoadLanguages();
        }

        if (allLanguages.Count > 0)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Select Language to Edit", EditorStyles.boldLabel);

            // Dropdown to select a language to edit
            selectedLanguageIndex = EditorGUILayout.Popup("Language", selectedLanguageIndex, languageNames);

            // Display the selected language's key-value pairs for editing
            LanguageData selectedLanguage = allLanguages[selectedLanguageIndex];
            DisplayLanguageEditor(selectedLanguage);

            EditorGUILayout.Space(10);
        }

        if (masterLanguage != null)
        {
            EditorGUILayout.LabelField("Master Language Keys", EditorStyles.boldLabel);
            DisplayMasterLanguageEditor();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Sync Keys to All Languages"))
            {
                SyncKeysWithFolder();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign a Master Language.", MessageType.Warning);
        }
    }

    private void LoadLanguages()
    {
        if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder))
        {
            Debug.LogError("Target folder does not exist or is invalid.");
            allLanguages.Clear();
            languageNames = new string[0];
            return;
        }

        // Load all LanguageData assets in the folder
        string[] assetPaths = Directory.GetFiles(targetFolder, "*.asset", SearchOption.AllDirectories);
        allLanguages.Clear();

        foreach (var path in assetPaths)
        {
            var language = AssetDatabase.LoadAssetAtPath<LanguageData>(path);
            if (language != null)
            {
                allLanguages.Add(language);
            }
        }

        // Update language names for the dropdown
        languageNames = new string[allLanguages.Count];
        for (int i = 0; i < allLanguages.Count; i++)
        {
            languageNames[i] = allLanguages[i].languageName;
        }

        if (allLanguages.Count > 0)
        {
            Debug.Log($"Loaded {allLanguages.Count} languages from {targetFolder}.");
        }
        else
        {
            Debug.LogWarning("No LanguageData assets found in the specified folder.");
        }
    }

    private void DisplayLanguageEditor(LanguageData language)
    {
        if (language == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Display and modify key-value pairs in the selected language
        for (int i = 0; i < language.entries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Key field and Value field
            language.entries[i].key = EditorGUILayout.TextField("Key", language.entries[i].key);
            language.entries[i].value = EditorGUILayout.TextField("Value", language.entries[i].value);

            // Delete button
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                language.entries.RemoveAt(i);
                EditorUtility.SetDirty(language);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private string GenerateUniqueKey(string baseKey, LanguageData language)
    {
        string newKey = baseKey;
        int counter = 1;

        // Ensure the new key is unique within the selected language
        while (language.entries.Exists(entry => entry.key == newKey))
        {
            newKey = $"{baseKey}_{counter}";
            counter++;
        }

        return newKey;
    }

    private void DisplayMasterLanguageEditor()
    {
        if (masterLanguage == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Display only keys in the master language
        for (int i = 0; i < masterLanguage.entries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Key field
            masterLanguage.entries[i].key = EditorGUILayout.TextField("Key", masterLanguage.entries[i].key);

            // Delete button
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                masterLanguage.entries.RemoveAt(i);
                EditorUtility.SetDirty(masterLanguage);
                SyncKeysWithFolder();
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add New Key"))
        {
            AddNewKeyToMaster();
        }
    }

    private void AddNewKeyToMaster()
    {
        string newKey = GenerateUniqueKey("NewKey", masterLanguage);
        AddKeyToLanguage(masterLanguage, newKey);
        SyncKeysWithFolder();
    }

    private void SyncKeysWithFolder()
    {
        if (masterLanguage == null)
        {
            Debug.LogError("Master Language is not assigned.");
            return;
        }

        if (allLanguages.Count == 0)
        {
            Debug.LogError("No languages loaded to sync.");
            return;
        }

        // Get the keys from the master language
        HashSet<string> masterKeys = new HashSet<string>();
        foreach (var entry in masterLanguage.entries)
        {
            masterKeys.Add(entry.key);
        }

        foreach (var language in allLanguages)
        {
            if (language == masterLanguage) continue;
            SyncLanguageKeys(language, masterKeys);
        }

        Debug.Log($"Synced keys from {masterLanguage.languageName} to all loaded languages.");
    }

    private void SyncLanguageKeys(LanguageData language, HashSet<string> masterKeys)
    {
        HashSet<string> currentKeys = new HashSet<string>();
        foreach (var entry in language.entries)
        {
            currentKeys.Add(entry.key);
        }

        // Add missing keys
        foreach (var key in masterKeys)
        {
            if (!currentKeys.Contains(key))
            {
                language.entries.Add(new LocalizationEntry { key = key, value = "[Missing Translation]" });
                Debug.Log($"Added missing key '{key}' to {language.languageName}");
            }
        }

        // Remove extra keys
        language.entries.RemoveAll(entry => !masterKeys.Contains(entry.key));
        EditorUtility.SetDirty(language);
    }

    private void AddKeyToLanguage(LanguageData language, string newKey)
    {
        // Check if the key already exists in the language
        if (language.entries.Exists(entry => entry.key == newKey))
        {
            Debug.LogWarning($"Key '{newKey}' already exists in {language.languageName}.");
            return;
        }

        // Add the new key to the language
        language.entries.Add(new LocalizationEntry { key = newKey, value = "[New Translation]" });
        EditorUtility.SetDirty(language);
    }
}
