using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCtrl : MonoBehaviour
{
    [SerializeField] private Button m_Play;
    [SerializeField] private Button m_Settings;
    [SerializeField] private Button m_Quit;

    [SerializeField] private GameObject m_SettingsContainer;
    [SerializeField] private Button m_CloseSettings;

    [SerializeField] private string m_SceneToLoadOnPlay;

    private void OnEnable() {
        if (m_Play) m_Play.onClick.AddListener(Play);
        if (m_Quit) m_Quit.onClick.AddListener(Quit);

        if (m_Settings) m_Settings.onClick.AddListener(OpenSettings);
        if (m_CloseSettings) m_CloseSettings.onClick.AddListener(CloseSettings);
    }
    private void OnDisable() {
        if (m_Play) m_Play.onClick.RemoveAllListeners();
        if (m_Quit) m_Quit.onClick.RemoveAllListeners();
        if (m_Settings) m_Settings.onClick.RemoveAllListeners();
        if (m_CloseSettings) m_CloseSettings.onClick.RemoveAllListeners();
    }

    private void Start() {
        CloseSettings();
    }

    public void Play() {
        // Load game
        SceneManager.LoadScene(m_SceneToLoadOnPlay);
    }

    public void Quit() {
        Application.Quit();
    }

    public void OpenSettings() {
        if (!m_SettingsContainer) return;
        m_SettingsContainer.SetActive(true);
    }

    public void CloseSettings() {
        if (!m_SettingsContainer) return;
        m_SettingsContainer.SetActive(false);
    }
}
