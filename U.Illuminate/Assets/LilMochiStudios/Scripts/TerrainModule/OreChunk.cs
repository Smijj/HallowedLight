using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public class OreChunk : Chunk {

        public override void InitializeGrid(MaterialDataSO materialData) {
            m_SquareGrid = new SquareGrid(m_GridSize, m_GridScale, m_IsoValue);
            //m_SquareGrid.GenerateCircleGridData(m_GridSize, materialData.SizeRange);
            m_SquareGrid.GenerateRandomShapeGridData(m_GridSize, materialData.SizeRange);
            GenerateMesh();
        }

    }
}
