using LilMochiStudios.TerrainModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.CoreModule {
    public class QuotaManager : MonoBehaviour
    {
        [SerializeField] private List<QuotaItem> m_Quota = new List<QuotaItem>();
        [SerializeField] private int m_QuotaMax = 50;
        [SerializeField] private int m_QuotaMin = 10;

        #region Unity
        private void OnEnable() {
            States.QuotaState.OnAddToQuota += AddItemToQuota;
            States.QuotaState.OnRemoveFromQuota += RemoveItemFromQuota;
            States.QuotaState.OnGetQuotaItem += OnGetQuotaItem;
        }
        private void OnDisable() {
            States.QuotaState.OnAddToQuota -= AddItemToQuota;
            States.QuotaState.OnRemoveFromQuota -= RemoveItemFromQuota;
            States.QuotaState.OnGetQuotaItem -= OnGetQuotaItem;
        }
        private void Start() {
            InitializeQuota();
        }
        private void InitializeQuota() {
            if (m_Quota.Count.Equals(0)) return;
            foreach (var quota in m_Quota) {
                quota.Quota = Random.Range(m_QuotaMin, m_QuotaMax);
            }

            States.QuotaState.OnQuotaChanged?.Invoke(m_Quota);
        }
        #endregion

        #region Events
        private void AddItemToQuota(MaterialDataSO material) {
            if (m_Quota.Count.Equals(0)) return;
            foreach (var quota in m_Quota) {
                // Material is in Quota
                if (quota.Material == material) {
                    quota.Collected++;
                    States.QuotaState.OnQuotaChanged?.Invoke(m_Quota);
                    
                    if (IsQuotaIsAchieved()) {
                        States.QuotaState.OnQuotaReached?.Invoke();
                    }
                }
            }
        }
        private void RemoveItemFromQuota(MaterialDataSO material) {
            if (m_Quota.Count.Equals(0)) return;
            foreach (var quota in m_Quota) {
                // Material is in Quota
                if (quota.Material == material) {
                    quota.Collected--;
                    States.QuotaState.OnQuotaChanged?.Invoke(m_Quota);
                }
            }
        }

        private QuotaItem OnGetQuotaItem(MaterialDataSO quotaMaterial) {
            if (m_Quota.Count.Equals(0)) return null;
            
            foreach (var quota in m_Quota) {
                // Material is in Quota
                if (quota.Material == quotaMaterial) {
                    return quota;
                }
            }
            return null;
        }
        #endregion

        #region Private
        private bool IsQuotaIsAchieved() {
            int quotaItemDone = 0;
            foreach (var item in m_Quota) {
                if (item.Collected >= item.Quota) {
                    quotaItemDone++;
                }
            }
            return quotaItemDone >= m_Quota.Count;
        }
        #endregion

        [System.Serializable]
        public class QuotaItem {
            public MaterialDataSO Material;
            public int Quota;
            public int Collected;
        }
    }
}
