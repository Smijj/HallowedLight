using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LilMochiStudios.UIModule {
    public class QuotaReachedUICtrl : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float m_ExtractHoldTime = 1.5f;
        [ReadOnly, SerializeField] private float m_ExtractHoldTimeCounter = 0;

        [Header("Elements")]
        [SerializeField] private GameObject m_Container;
        [SerializeField] private Image m_HoldFillImage;

        private bool m_Visible = false;

        private void OnEnable() {
            CoreModule.States.QuotaState.OnQuotaReached += OnQuotaReached;
        }
        private void OnDisable() {
            CoreModule.States.QuotaState.OnQuotaReached -= OnQuotaReached;
        }
        private void Start() {
            Hide();
        }
        private void Update() {
            HandleExtractButton();
        }


        private void OnQuotaReached() {
            Show();
        }

        private void HandleExtractButton() {
            if (!m_Visible) return;

            if (Input.GetKey(KeyCode.E)) {
                m_ExtractHoldTimeCounter += Time.deltaTime;
            } else {
                m_ExtractHoldTimeCounter = 0;
            }

            // Fill hold image based on how long the extract button has been pressed
            if (m_HoldFillImage) {
                m_HoldFillImage.fillAmount = m_ExtractHoldTimeCounter / m_ExtractHoldTime;
            }

            if (m_ExtractHoldTimeCounter >= m_ExtractHoldTime) {
                // Extract
                CoreModule.States.LevelState.OnExtract?.Invoke();
            }
        }

        private void Show() {
            if (!m_Container) return;
            m_Container.SetActive(true);
            m_Visible = true;
        }
        private void Hide() {
            if (!m_Container) return;
            m_Container.SetActive(false);
            m_Visible = false;
        }
    }
}
