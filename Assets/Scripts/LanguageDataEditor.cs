using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LanguageData))]
public class LanguageDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LanguageData languageData = (LanguageData)target;

        EditorGUILayout.LabelField($"Editing Language: {languageData.languageName}", EditorStyles.boldLabel);

        if (GUILayout.Button("Add New Entry"))
        {
            languageData.entries.Add(new LocalizationEntry());
        }

        for (int i = 0; i < languageData.entries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            languageData.entries[i].key = EditorGUILayout.TextField("Key", languageData.entries[i].key);
            languageData.entries[i].value = EditorGUILayout.TextField("Value", languageData.entries[i].value);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                languageData.entries.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Sort Entries"))
        {
            languageData.entries.Sort((a, b) => a.key.CompareTo(b.key));
        }

        EditorUtility.SetDirty(languageData);
    }
}
