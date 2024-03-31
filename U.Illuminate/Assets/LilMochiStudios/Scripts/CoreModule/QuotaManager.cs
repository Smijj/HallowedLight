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

        private void OnEnable() {
            States.QuotaState.OnAddToQuota += AddItemToQuota;
            States.QuotaState.OnRemoveFromQuota += RemoveItemFromQuota;
        }
        private void OnDisable() {
            States.QuotaState.OnAddToQuota -= AddItemToQuota;
            States.QuotaState.OnRemoveFromQuota -= RemoveItemFromQuota;
        }

        private void Start() {
            InitializeQuota();
        }

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

        

        private void InitializeQuota() {
            if (m_Quota.Count.Equals(0)) return;
            foreach (var quota in m_Quota) {
                quota.Quota = Random.Range(m_QuotaMin, m_QuotaMax);
            }

            States.QuotaState.OnQuotaChanged?.Invoke(m_Quota);
        }

        private bool IsQuotaIsAchieved() {
            int quotaItemDone = 0;
            foreach (var item in m_Quota) {
                if (item.Collected >= item.Quota) {
                    quotaItemDone++;
                }
            }
            return quotaItemDone >= m_Quota.Count;
        }

        [System.Serializable]
        public class QuotaItem {
            public MaterialDataSO Material;
            public int Quota;
            public int Collected;
        }
    }
}
