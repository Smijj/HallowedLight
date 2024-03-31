using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public class DestructableDropsManager : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private DestructableDropCtrl m_OreDropPrefab;
        [SerializeField] private Transform m_OreMagnetTarget;

        private void OnEnable() {
            States.DestructableState.OnDestructableDropItem += SpawnOreDrop;
        }
        private void OnDisable() {
            States.DestructableState.OnDestructableDropItem -= SpawnOreDrop;
        }

        private void SpawnOreDrop(MaterialDataSO material, Vector2 spawnPosition) {
            DestructableDropCtrl oreDrop = Instantiate(m_OreDropPrefab, spawnPosition, Quaternion.identity, transform);
            oreDrop.Initialize(m_OreMagnetTarget, material);
        }
    }
}
