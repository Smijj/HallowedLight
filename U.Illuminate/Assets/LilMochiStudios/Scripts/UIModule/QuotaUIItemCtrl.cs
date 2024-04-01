using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LilMochiStudios.UIModule {
    public class QuotaUIItemCtrl : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Text;

        public void SetIcon(Sprite icon) {
            if (icon == null || !m_Icon) return;
            m_Icon.sprite = icon;
        }

        public void SetText(string text) {
            if (text == null || !m_Text) return;
            m_Text.text = text;
        }
    }
}
