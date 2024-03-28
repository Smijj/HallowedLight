using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{

    [Header("Brush Settings")]
    [SerializeField] private int m_BrushRadius = 2;
    [SerializeField] private float m_BrushStrength = 0.5f;
    [SerializeField] private float m_BrushFalloff = 4f;

    [Header("Elements")]
    [SerializeField] private MeshFilter m_MeshFilter;
    [SerializeField] protected MeshRenderer m_MeshRenderer;
    protected Mesh m_Mesh;

    [Header("Data")]
    [SerializeField] protected int m_GridSize;
    [SerializeField] protected float m_GridScale;
    [SerializeField] protected float m_UVScale;

    protected SquareGrid m_SquareGrid;


    private void OnEnable() {
        InputManager.OnTouching += TouchingCallback;
    }
    private void OnDisable() {
        InputManager.OnTouching -= TouchingCallback;
    }

    public virtual void Initialize(int gridSize, float gridScale, float isoValue, Material material) {
        this.m_GridSize = gridSize;
        this.m_GridScale = gridScale;

        m_Mesh = new Mesh();
        if (m_MeshRenderer) m_MeshRenderer.material = material;

        m_SquareGrid = new SquareGrid(m_GridSize, m_GridScale, isoValue);

        GenerateMesh();
    }

    private void TouchingCallback(Vector3 worldPosition) {
        worldPosition.z = 0f;
        
        // For spliting the terrain meshes into chunks, it converts the world space pos to a local pos to effect the correct mesh.
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
                m_SquareGrid.GridData[currentGridPos.x, currentGridPos.y] -= factor;

                // This is to make sure that not all of the terrain meshes in different chunks update even if you clicked nowhere near them.
                shouldGenerate = true;
            }
        }

        if (shouldGenerate)
            GenerateMesh();
    }

    protected void GenerateMesh() {
        m_Mesh = new Mesh();

        m_SquareGrid.Update();

        m_Mesh.vertices = m_SquareGrid.GetVertices();
        m_Mesh.triangles = m_SquareGrid.GetTriangles();

        // Offset UVs so that textures applied to the terrain are correctly displayed. The order these operations happen is is very important!
        Vector2[] uvs = m_SquareGrid.GetUVs();
        for (int i = 0; i < uvs.Length; i++) {
            uvs[i] /= m_GridSize-1;                             // This scales the uv to be the same size as the chunks mesh. Making the texture be the same size as the chunk
            uvs[i] += Vector2.one * this.m_GridScale / 2;       // This offsets the uv to make the texture lineup with the mesh
            uvs[i] /= this.m_GridScale;                         // This scales the uv to account for the GridScale
            uvs[i] /= m_UVScale;                                // Value that allows for changing the tiling of the UV's from the inspector
        }
        m_Mesh.uv = uvs;

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

    private Vector2Int GetGridPositionFromWorldPosition(Vector2 worldPosition) {
        Vector2Int gridPos = new Vector2Int();
        gridPos.x = Mathf.FloorToInt(worldPosition.x / m_GridScale + m_GridSize / 2 - m_GridScale / 2);
        gridPos.y = Mathf.FloorToInt(worldPosition.y / m_GridScale + m_GridSize / 2 - m_GridScale / 2);
        return gridPos;
    }
}
