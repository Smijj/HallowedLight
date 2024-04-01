using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class SettingsHandler {
    private static readonly string m_DefaultSettingsResourcePath = "Settings/DefaultSettings";
    private static SettingsSO DefaultSettings;  // Dont use this one, its just to store the asset in memory
    private static SettingsSO m_DefaultSettings {
        get {
            if (DefaultSettings != null) {
                return DefaultSettings;
            }

            SettingsSO settingsSO = Resources.Load<SettingsSO>(m_DefaultSettingsResourcePath);
            if (settingsSO == null)
                settingsSO = ScriptableObject.CreateInstance<SettingsSO>();

            DefaultSettings = settingsSO;
            return settingsSO;
        }
    }

    private static List<string> m_ActiveKeys = new List<string>();

    public static System.Action OnSettingsResetToDefault;


    #region Audio Settings =========================

    public static float GetAudioValue(string key) {
        return GetFloat(key, m_DefaultSettings.DefaultVolume);
    }
    public static void SetAudioValue(string key, float value) {
        SetFloat(key, value);
    }

    #endregion


    #region Public Functions

#if UNITY_EDITOR
    [MenuItem("LarrikinInteractive/Settings/ResetSettingsToDefault")]
#endif
    public static void ResetAllToDefault() {
        if (m_ActiveKeys.Count.Equals(0)) return;

        // If there is no key when the value is requested then it will give the default value
        foreach (var key in m_ActiveKeys) {
            PlayerPrefs.DeleteKey(key);
        }
        m_ActiveKeys.Clear();
        OnSettingsResetToDefault?.Invoke();
    }

    #endregion


    #region Getters & Setters

    private static bool GetBool(string key, bool defaultValue = false) {
        AddKey(key);
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0).Equals(1);    // Converts to bool by checking if it equals 1, if it does its true otherwise its false.
    }
    private static void SetBool(string key, bool value) {
        AddKey(key);
        PlayerPrefs.SetInt(key, value ? 1 : 0);   // if value is true save it as a 1, otherwise save it as a 0
        PlayerPrefs.Save();  
    }
    private static int GetInt(string key, int defaultValue = 0) {
        AddKey(key);
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    private static void SetInt(string key, int value) {
        AddKey(key);
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();  
    }
    private static float GetFloat(string key, float defaultValue = 0f) {
        AddKey(key);
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    private static void SetFloat(string key, float value) {
        AddKey(key);
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();  
    }
    private static string GetString(string key, string defaultValue = "", bool persistantData = false) {
        if (!persistantData) AddKey(key);
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        return PlayerPrefs.GetString(key, defaultValue);
    }
    private static void SetString(string key, string value, bool persistantData = false) {
        if (!persistantData) AddKey(key);
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();  
    }

    /// <summary>
    /// Whenever a player pref value is attempts to be Get or Set then add that key to the list if its not already there
    /// </summary>
    /// <param name="key"></param>
    private static void AddKey(string key) {
        if (!m_ActiveKeys.Contains(key)) m_ActiveKeys.Add(key);
    }

    #endregion
}
