using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    [Header("Data")]
    [SerializeField] private int m_GridSize;
    [SerializeField] private float m_GridScale;
    private float[,] m_Grid;
    
    [Header("Settings")]
    [SerializeField] private bool m_EnableGizmos = false;
    [SerializeField] private float m_GizmosSize = 0.5f;


    private void Start() {
        m_Grid = new float[m_GridSize, m_GridSize];
    }


    private Vector2 GetWorldPositionFromGridPosition(int x, int y) {
        Vector2 worldPosition = new Vector2(x, y) * m_GridScale;
        worldPosition.x -= (m_GridSize * m_GridScale) / 2 - m_GridScale / 2;
        worldPosition.y -= (m_GridSize * m_GridScale) / 2 - m_GridScale / 2;
        

        return worldPosition;
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying || !m_EnableGizmos) return;

        Gizmos.color = Color.red;

        for (int y = 0;  y < m_Grid.GetLength(1); y++) { 
            for (int x = 0; x < m_Grid.GetLength(0); x++) {
                //Vector2 gridPosition = new Vector2(x, y);
                Vector2 worldPosition = GetWorldPositionFromGridPosition(x, y);
                Gizmos.DrawSphere(worldPosition, m_GizmosSize);
            }
        }
    }
}
