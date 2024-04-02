using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LilMochiStudios.CoreModule {
    public class LevelManager : MonoBehaviour
    {
        // Handles Initalizing level and finishing level

        [SerializeField] private string m_SceneToLoadOnExtract = "MainMenu";



        private void OnEnable() {
            States.LevelState.OnExtract += OnExtract;
        }
        private void OnDisable() {
            States.LevelState.OnExtract -= OnExtract;
        }

        private void OnExtract() {
            SceneManager.LoadScene(m_SceneToLoadOnExtract);
        }
    }
}
