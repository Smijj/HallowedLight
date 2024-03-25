using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainGenerator : MonoBehaviour
{

    [Header("Brush Settings")]
    [SerializeField] private int m_BrushRadius = 2;
    [SerializeField] private float m_BrushStrength = 0.5f;
    [SerializeField] private float m_BrushFalloff = 4f;

    [Header("Elements")]
    [SerializeField] private MeshFilter m_MeshFilter;
    private Mesh m_Mesh;

    [Header("Data")]
    [SerializeField] private int m_GridSize;
    [SerializeField] private float m_GridScale;
    [SerializeField] private float m_IsoValue;

    private SquareGrid m_SquareGrid;
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
        Application.targetFrameRate = 60;
        m_Mesh = new Mesh();

        // TODO: Put into the SquareGrid struct
        m_Grid = new float[m_GridSize, m_GridSize];
        for (int y = 0; y < m_GridSize; y++) {
            for (int x = 0; x < m_GridSize; x++) {
                m_Grid[x, y] = m_IsoValue + 0.1f;
            }
        }

        m_SquareGrid = new SquareGrid(m_GridSize - 1, m_GridScale, m_IsoValue);

        GenerateMesh();
    }

    private void TouchingCallback(Vector3 worldPosition) {
        worldPosition.z = 0f;
        
        // For spliting the terrain meshes into chunks, it covnerts the world space pos to a local pos to effect the correct mesh.
        worldPosition = transform.InverseTransformPoint(worldPosition); 
        Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        bool shouldGenerate = false;

        for (int y = gridPosition.y - m_BrushRadius / 2; y <= gridPosition.y + m_BrushRadius / 2; y++) {
            for (int x = gridPosition.x - m_BrushRadius / 2; x <= gridPosition.x + m_BrushRadius / 2; x++) {
                Vector2Int currentGridPos = new Vector2Int (x, y);
                
                // Check if girs pos is valid, if not skip this grid pos
                if (!IsValidGridPosition(currentGridPos)) continue;
                
                // Calculate how much we should edit a particular grid point based on the distance from where the player clicked
                float distance = Vector2.Distance(currentGridPos, gridPosition);
                float factor = Mathf.Exp(-distance * m_BrushFalloff / m_BrushRadius) * m_BrushStrength;

                // Modify the grid point
                m_Grid[currentGridPos.x, currentGridPos.y] -= factor;

                // This is to make sure that not all of the terrain meshes in different chunks update even if you clicked nowhere near them.
                shouldGenerate = true;
            }
        }

        if (shouldGenerate)
            GenerateMesh();
    }

    private void GenerateMesh() {
        m_Mesh = new Mesh();

        m_SquareGrid.Update(m_Grid);

        m_Mesh.vertices = m_SquareGrid.GetVertices();
        m_Mesh.triangles = m_SquareGrid.GetTriangles();

        m_MeshFilter.mesh = m_Mesh;

        GenerateCollider();
    }

    private void GenerateCollider() {
        if (m_MeshFilter.TryGetComponent(out MeshCollider meshCollider))
            meshCollider.sharedMesh = m_Mesh;
        else
            m_MeshFilter.AddComponent<MeshCollider>().sharedMesh = m_Mesh;
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
