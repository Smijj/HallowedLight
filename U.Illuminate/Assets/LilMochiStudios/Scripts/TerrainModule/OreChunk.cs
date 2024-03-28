using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreChunk : TerrainChunk
{


    public override void Initialize(int gridSize, float gridScale, float isoValue, Material material) {
        this.m_GridSize = gridSize;
        this.m_GridScale = gridScale;

        m_Mesh = new Mesh();
        if (m_MeshRenderer) m_MeshRenderer.material = material;

        m_SquareGrid = new SquareGrid(m_GridSize, m_GridScale, isoValue);
        m_SquareGrid.GenerateCircleGridData(m_GridSize);

        GenerateMesh();
    }

}
