using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LilMochiStudios.CoreModule.QuotaManager;

namespace LilMochiStudios.UIModule {
    public class QuotaUICtrl : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Transform m_QuotaItemListParent;
        [SerializeField] private QuotaUIItemCtrl m_QuotaItemPrefab;

        private List<QuotaUIItemCtrl> m_QuotaUIItems = new List<QuotaUIItemCtrl>();

        private void OnEnable() {
            CoreModule.States.QuotaState.OnQuotaChanged += OnQuotaChanged;
        }
        private void OnDisable() {
            CoreModule.States.QuotaState.OnQuotaChanged -= OnQuotaChanged;
        }

        private void OnQuotaChanged(List<QuotaItem> quotaList) {
            UpdateQuotaUIList(quotaList);
        }

        private void UpdateQuotaUIList(List<QuotaItem> quotaList) {
            // Clear UI list
            foreach (var quotaUIItem in m_QuotaUIItems) {
                Destroy(quotaUIItem.gameObject);
            }
            m_QuotaUIItems.Clear();

            if (quotaList == null || quotaList.Count.Equals(0)) return;

            // Add items and set their values
            foreach (var quotaItem in quotaList) {
                QuotaUIItemCtrl quotaUIItem = Instantiate(m_QuotaItemPrefab, m_QuotaItemListParent);
                quotaUIItem.SetIcon(quotaItem.Material.DropSprites[0]);
                quotaUIItem.SetText($"{quotaItem.Material.name} - {quotaItem.Collected}/{quotaItem.Quota}");
                m_QuotaUIItems.Add(quotaUIItem);
            }

        }
    }
}
