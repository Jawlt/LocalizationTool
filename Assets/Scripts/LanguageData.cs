using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Language", menuName = "Localization/Language Data")]
public class LanguageData : ScriptableObject
{
    [SerializeField]
    public string languageName;
    public List<LocalizationEntry> entries = new List<LocalizationEntry>();
}

[System.Serializable]
public class LocalizationEntry
{
    public string key;
    public string value;
}
