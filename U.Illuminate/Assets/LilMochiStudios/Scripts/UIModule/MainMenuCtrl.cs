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

    [SerializeField] private string m_SceneToLoadOnPlay;

    private void OnEnable() {
        if (m_Play) m_Play.onClick.AddListener(Play);
        if (m_Quit) m_Quit.onClick.AddListener(Quit);
    }
    private void OnDisable() {
        if (m_Play) m_Play.onClick.RemoveAllListeners();
        if (m_Quit) m_Quit.onClick.RemoveAllListeners();
    }

    public void Play() {
        // Load game
        SceneManager.LoadScene(m_SceneToLoadOnPlay);
    }

    public void Quit() {
        Application.Quit();
    }
}
