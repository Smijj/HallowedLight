using LilMochiStudios.TerrainModule.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public class OreChunk : Chunk {

        [Header("Ore Settings")]
        [SerializeField] private float m_PercentageMined = 0f;
        [SerializeField] private float m_LastDropPercentage = 0f;

        public override void InitializeGrid(MaterialDataSO materialData) {
            m_SquareGrid = new SquareGrid(m_GridSize, m_GridScale, m_IsoValue);
            
            switch (m_MaterialData.SpawnShapeType) {
                case 0:
                    break;
                case 1:
                    m_SquareGrid.GenerateCircleGridData(m_GridSize, materialData.SizeRange);
                    break;
                case 2:
                    m_SquareGrid.GenerateRandomShapeGridData(m_GridSize, materialData.SizeRange);
                    break;
            }

            GenerateMesh();
        }

        public override void DestroyTerrain(Vector2Int gridPosition) {
            base.DestroyTerrain(gridPosition);

            m_PercentageMined = this.m_GridValueRemoved / this.m_SquareGrid.TotalGridDataValue;
            if (m_MaterialData.CanDropItem && m_PercentageMined - m_LastDropPercentage > m_MaterialData.DropRate) {
                // drop ore
                Vector2 targetPoition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                DestructableState.OnDestructableDropItem?.Invoke(m_MaterialData, targetPoition);

                m_LastDropPercentage = m_PercentageMined;
            }
        }
    }
}
