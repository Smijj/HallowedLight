using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainGenerator : MonoBehaviour
{

    [Header("Brush Settings")]
    [SerializeField] private int m_BrushRadius;
    [SerializeField] private float m_BrushStrength;

    [Header("Data")]
    [SerializeField] private int m_GridSize;
    [SerializeField] private float m_GridScale;
    [SerializeField] private float m_IsoValue;

    private float[,] m_Grid;
    
    [Header("Settings")]
    [SerializeField] private bool m_EnableGizmos = false;
    [SerializeField] private float m_GizmosSize = 0.5f;


    private void OnEnable() {
        InputManager.OnTouching += TouchingCallback;
    }
    private void OnDisable() {
        InputManager.OnTouching -= TouchingCallback;
    }

    private void Start() {
        m_Grid = new float[m_GridSize, m_GridSize];

        for (int y = 0; y < m_GridSize; y++) {
            for (int x = 0; x < m_GridSize; x++) {
                m_Grid[x, y] = m_IsoValue + 0.1f;
            }
        }
    }

    private void TouchingCallback(Vector3 worldPosition) {
        worldPosition.z = 0f;
        Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        //if (!IsValidGridPosition(gridPosition)) {
        //    Debug.LogWarning("Invalid Grid Pos");
        //    return;
        //}

        for (int y = gridPosition.y - m_BrushRadius / 2; y <= gridPosition.y + m_BrushRadius / 2; y++) {
            for (int x = gridPosition.x - m_BrushRadius / 2; x <= gridPosition.x + m_BrushRadius / 2; x++) {
                Vector2Int currentGridPos = new Vector2Int (x, y);
                
                if (!IsValidGridPosition(currentGridPos)) {
                    Debug.LogWarning("Invalid Grid Pos");
                    continue;
                }

                m_Grid[currentGridPos.x, currentGridPos.y] -= m_BrushStrength;
            }
        }
    }

    private bool IsValidGridPosition(Vector2Int gridPosition) {
        return gridPosition.x >= 0f && gridPosition.x < m_GridSize && gridPosition.y >= 0f && gridPosition.y < m_GridSize;
    }

    private Vector2 GetWorldPositionFromGridPosition(int x, int y) {
        Vector2 worldPosition = new Vector2(x, y) * m_GridScale;
        worldPosition.x -= (m_GridSize * m_GridScale) / 2 - m_GridScale / 2;
        worldPosition.y -= (m_GridSize * m_GridScale) / 2 - m_GridScale / 2;
        return worldPosition;
    }
    private Vector2Int GetGridPositionFromWorldPosition(Vector2 worldPosition) {
        Vector2Int gridPos = new Vector2Int();
        gridPos.x = Mathf.FloorToInt(worldPosition.x / m_GridScale + m_GridSize / 2 - m_GridScale / 2);
        gridPos.y = Mathf.FloorToInt(worldPosition.y / m_GridScale + m_GridSize / 2 - m_GridScale / 2);
        return gridPos;
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying || !m_EnableGizmos) return;

        Gizmos.color = Color.red;

        for (int y = 0;  y < m_Grid.GetLength(1); y++) { 
            for (int x = 0; x < m_Grid.GetLength(0); x++) {
                Vector2 worldPosition = GetWorldPositionFromGridPosition(x, y);
                Gizmos.DrawSphere(worldPosition, m_GridScale / 4);

#if UNITY_EDITOR
                Handles.Label(worldPosition + Vector2.up * m_GridScale / 3, m_Grid[x, y].ToString("0.0"));
#endif
            }
        }
    }
}
