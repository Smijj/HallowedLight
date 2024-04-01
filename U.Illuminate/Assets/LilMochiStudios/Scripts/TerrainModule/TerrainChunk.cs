using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public class TerrainChunk : Chunk {

        public override void InitializeGrid(MaterialDataSO materialData) {
            base.InitializeGrid(materialData);
            //m_SquareGrid = new SquareGrid(m_GridSize, m_GridScale, m_IsoValue);
            //m_SquareGrid.GenerateCircleGridData(m_GridSize, materialData.SizeRange);
            //GenerateMesh();
        }

    }
}
