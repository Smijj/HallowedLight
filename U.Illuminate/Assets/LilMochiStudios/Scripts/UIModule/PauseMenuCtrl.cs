using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuCtrl : MonoBehaviour
{
    [SerializeField] private string m_SceneToLoadOnQuit = "MainMenu";
    
    [SerializeField] private Button m_Resume;
    [SerializeField] private Button m_Quit;
    [SerializeField] private GameObject m_PauseMenuContainer;

    public static bool Paused = false;

    private void OnEnable() {
        if (m_Resume) m_Resume.onClick.AddListener(Resume);
        //if (m_Settings) m_Settings.onClick.AddListener(OpenSettings);
        if (m_Quit) m_Quit.onClick.AddListener(Quit);

    }
    private void OnDisable() {
        if (m_Resume) m_Resume.onClick.RemoveAllListeners();
        //if (m_Settings) m_Settings.onClick.RemoveAllListeners();
        if (m_Quit) m_Quit.onClick.RemoveAllListeners();
    }

    private void Start() {
        ClosePauseMenu();
    }

    private void Update() {
        if (!Paused && Input.GetKeyDown(KeyCode.Escape)) OpenPauseMenu();
    }

    public void Resume() {
        ClosePauseMenu();
    }

    public void Quit() {
        SceneManager.LoadScene(m_SceneToLoadOnQuit);
    }

    public void OpenPauseMenu() {
        if (!m_PauseMenuContainer) return;
        m_PauseMenuContainer.SetActive(true);
        Time.timeScale = 0;
        Paused = true;
    }

    public void ClosePauseMenu() {
        if (!m_PauseMenuContainer) return;
        m_PauseMenuContainer.SetActive(false);
        Time.timeScale = 1;
        Paused = false;
    }
}
